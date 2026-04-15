using Components;
using HECSFramework.Core;

public class FollowState : LooterDroneState
{
    public FollowState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
    }

    public override int StateID => FollowState;

    public override void Enter(Entity entity)
    {
        entity.AddComponent<DroneFollowTagComponent>();
    }

    public override void Exit(Entity entity)
    {
        entity.RemoveComponent<DroneFollowTagComponent>();
    }

    public override void Update(Entity entity)
    {
        stateMachine.TryToNextStateByTransition();
        
        if (entity.World.TryGetSingleComponent(out PlayerCharacterComponent playerCharacterComponent))
        {
            var characterEntity = playerCharacterComponent.Owner;
            var velocityComponent = entity.GetComponent<DroneFollowVelocityComponent>();

            if (characterEntity.ContainsMask<OnStopMovingComponent>() && velocityComponent.Velocity.magnitude <= 0.1f)
                EndState();
        }
    }
}

public class IsResourcesNodeNear : ITransition
{
    private Entity droneEntity;

    public IsResourcesNodeNear(int toState, Entity drone)
    {
        ToState = toState;
        droneEntity = drone;
    }

    public int ToState { get; }

    public bool IsReady()
    {
        return droneEntity.TryGetComponent<CollectPointComponent>(out var compoent);
    }
}
