using Components;
using HECSFramework.Core;
using Systems;

public class TryFindAnotherLootState : LooterDroneState
{
    CurrentLootTargetComponent currentLootTargetComponent;
    private EntitiesFilter filter;

    public TryFindAnotherLootState(StateMachine stateMachine, int nextDefaultState, EntitiesFilter lootFilter, CurrentLootTargetComponent lootTargetComponent) : base(stateMachine, nextDefaultState)
    {
        filter = lootFilter;
        currentLootTargetComponent = lootTargetComponent;
    }

    public override int StateID => TryFindAnotherLootState;

    public override void Enter(Entity entity)
    {
        filter.ForceUpdateFilter();

        if (filter.Count > 0)
        {
            currentLootTargetComponent.CurrentCollectLootPosition = filter.FirstOrDefault().GetPosition();
        }

        EndState();
    }

    public override void Exit(Entity entity)
    {
    }

    public override void Update(Entity entity)
    {
    }

    public class IsNoLootInZone : ITransition
    {
        private EntitiesFilter filter;

        public IsNoLootInZone(int toState, EntitiesFilter lootFilter)
        {
            ToState = toState;
            filter = lootFilter;
        }

        public int ToState { get; }

        public bool IsReady()
        {
            return filter.Count == 0;
        }
    }
}
