using Components;
using HECSFramework.Core;
using UnityEngine;

public class RotateToStartPositionState : LooterDroneState
{
    private UnityTransformComponent ownerTransform;
    private RotationSpeedWhenCollectLootComponent rotationSpeedComponent;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float progress;

    public RotateToStartPositionState(StateMachine stateMachine, int nextDefaultState, UnityTransformComponent transformComponent,
        RotationSpeedWhenCollectLootComponent droneRotationSpeedComponent) : base(stateMachine, nextDefaultState)
    {
        ownerTransform = transformComponent;
        rotationSpeedComponent = droneRotationSpeedComponent;
    }

    public override int StateID => RotateToStartPositionState;

    public override void Enter(Entity entity)
    {
        var target = entity.GetComponent<DroneFollowTargetComponent>().FollowTarget;
        var direction = target.position - entity.GetPosition();
        var normalized = direction.normalized;

        startRotation = entity.GetRotation();
        targetRotation = Quaternion.LookRotation(normalized, Vector3.up);
    }

    public override void Exit(Entity entity)
    {

    }

    public override void Update(Entity entity)
    {
        stateMachine.TryToNextStateByTransition();

        progress += Time.deltaTime * rotationSpeedComponent.RotationSpeedWhenCollectLoot;

        var rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
        ownerTransform.Transform.rotation = rotation;

        if(progress >= 1)
        {
            startRotation = Quaternion.identity;
            targetRotation = Quaternion.identity;
            progress = 0;
            EndState();
        }
    }
}

public class IsDroneNearCharacter : ITransition
{
    private World world;

    public IsDroneNearCharacter(World world, int toState)
    {
        ToState = toState;
        this.world = world;
    }

    public int ToState { get; }

    public bool IsReady()
    {
        var drone = world.GetEntityBySingleComponent<LooterDroneTagComponent>();
        var config = drone.GetComponent<DroneLootCollectingConfigComponent>();
        var target = drone.GetComponent<DroneFollowTargetComponent>().FollowTarget;

        var sqrMagnitude = (drone.GetPosition() - target.position).sqrMagnitude;

        return sqrMagnitude <= config.CollectDistanceOffset;
    }
}
