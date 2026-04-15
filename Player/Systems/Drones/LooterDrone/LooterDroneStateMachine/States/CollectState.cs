using Components;
using Components.MonoBehaviourComponents;
using HECSFramework.Core;
using Commands;

public class CollectState : LooterDroneState
{
    private bool isUpdateActive;
    private HECSList<Entity> nodes = new HECSList<Entity>(8);
    private DroneCollectWhileFollowConfigComponent config;
    private Entity drone;

    private EntitiesFilter nodesFilter;

    public CollectState(StateMachine stateMachine, EntitiesFilter filter, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
        nodesFilter = filter;
    }

    public override int StateID => CollectState;

    public override void Enter(Entity entity)
    {
        drone = entity.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
        config = drone.GetComponent<DroneCollectWhileFollowConfigComponent>();
        nodes.ClearFast();

        var raidManager = entity.World.GetEntityBySingleComponent<RaidManagerTagComponent>();

        entity.World.Command(new UpdateLooterDronePositionCommand { DroneTransform = entity.GetTransform() });

        TryFindNearestNodes();

        foreach(var node in nodes )
        {
            node.GetComponent<RewardsLocalHolderComponent>().ExecuteRewards(new ExecuteReward
            {
                Owner = node,
                Target = raidManager
            });
        }

        isUpdateActive = true;
    }

    private void TryFindNearestNodes()
    {
        nodesFilter.ForceUpdateFilter();

        foreach (var node in nodesFilter)
        {
            if (node.ContainsMask<NodeCollectedTagComponent>())
                continue;

            var nodePosition = node.GetPosition();
            var sqrDistance = (nodePosition - drone.GetPosition()).sqrMagnitude;

            if (sqrDistance < config.SqrCollectDistance)
            {
                nodes.Add(node);
            }
        }
    }

    public override void Exit(Entity entity)
    {
    }

    public override void Update(Entity entity)
    {
        if (!isUpdateActive)
            return;

        if (entity.ContainsMask<VisualLocalLockComponent>())
            return;

        entity.RemoveComponent<CollectPointComponent>();
        isUpdateActive = false;
        EndState();
    }
}