using Components;
using HECSFramework.Core;
using Systems;

public class AwaitLootState : LooterDroneState
{
    private CurrentLootTargetComponent currentLootTargetComponent;
    private EntitiesFilter filter;

    public AwaitLootState(StateMachine stateMachine, int nextDefaultState, EntitiesFilter lootFilter, CurrentLootTargetComponent lootTargetComponent) : base(stateMachine, nextDefaultState)
    {
        filter = lootFilter;
        currentLootTargetComponent = lootTargetComponent;
    }

    public override int StateID => AwaitLootState;

    public override void Enter(Entity entity)
    {
    }

    public override void Exit(Entity entity)
    {       
    }

    public override void Update(Entity entity)
    {
        filter.ForceUpdateFilter();

        if (filter.Count > 0)
        {
            currentLootTargetComponent.CurrentCollectLootPosition = filter.FirstOrDefault().GetPosition();
            EndState();
        }

        stateMachine.TryToNextStateByTransition();
    }

    public class IsNeedToFollow : ITransition
    {
        private StateMachine shooterStateMachine;

        public IsNeedToFollow(World world, int toState)
        {
            ToState = toState;

            shooterStateMachine = world.GetSingleComponent<CoreFSMsComponent>().Shooter;
        }

        public int ToState { get; }

        public bool IsReady()
        {
            return (shooterStateMachine.CurrentState == ShooterState.MoveToShootingPointState || shooterStateMachine.CurrentState == ShooterState.MoveToExitState ||
                shooterStateMachine.CurrentState == ShooterState.MoveToPortalState);
        }
    }
}
