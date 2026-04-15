using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;

public class GetLootState : LooterDroneState
{
    private Entity drone;
    private HECSList<UniTask> taskList = new HECSList<UniTask>(8);
    private EntitiesFilter lootFilter;

    public GetLootState(StateMachine stateMachine, EntitiesFilter filter, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
        lootFilter = filter;
    }

    public override int StateID => GetLootState;

    public override void Enter(Entity entity)
    {
        if (taskList.Count > 0)
            taskList.ClearFast();

        drone = entity.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
        SearchAndCollectLootAsync(entity).Forget();
    }

    private async UniTask SearchAndCollectLootAsync(Entity entity)
    {
        entity.World.Command(new UpdateLooterDronePositionCommand { DroneTransform = entity.GetTransform() });
        var resourcesHolder = entity.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
        var config = entity.GetComponent<DroneLootCollectingConfigComponent>();

        lootFilter.ForceUpdateFilter();

        foreach(var loot in lootFilter)
        {
            var lootPosition = loot.GetPosition();
            var sqrDistance = (lootPosition - drone.GetPosition()).sqrMagnitude;

            if(sqrDistance < config.SqrScanRadius)
            {
                taskList.Add(GetLoot(entity, loot, resourcesHolder));
            }
        }

        await UniTask.WhenAll(taskList);
        await UniTask.NextFrame();

        EndState();
    }

    private async UniTask GetLoot(Entity ownerEntity, Entity lootEntity, Entity resourcesHolder)
    {
        lootEntity.GetComponent<RewardActorsHolderComponent>().ExecuteRewards(new ExecuteReward { Owner = lootEntity, Target = resourcesHolder });

        await new WaitRemove<VisualLocalLockComponent>(lootEntity).RunJob();
        ownerEntity.World.Command(new DestroyEntityWorldCommand { Entity = lootEntity });
    }

    public override void Exit(Entity entity)
    {

    }

    public override void Update(Entity entity)
    {

    }
}
