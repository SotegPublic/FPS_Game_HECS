using Components;
using HECSFramework.Core;
using UnityEngine;

public class RotateOnStartPositionState : LooterDroneState
{
    private UnityTransformComponent ownerTransform;
    private RotationSpeedWhenCollectLootComponent rotationSpeedComponent;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float progress;

    public RotateOnStartPositionState(StateMachine stateMachine, int nextDefaultState, UnityTransformComponent transformComponent,
        RotationSpeedWhenCollectLootComponent droneRotationSpeedComponent) : base(stateMachine, nextDefaultState)
    {
        ownerTransform = transformComponent;
        rotationSpeedComponent = droneRotationSpeedComponent;
    }

    public override int StateID => RotateOnStartPositionState;

    public override void Enter(Entity entity)
    {
        var target = entity.GetComponent<DroneFollowTargetComponent>().FollowTarget;

        startRotation = entity.GetRotation();
        targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
    }

    public override void Exit(Entity entity)
    {
    }

    public override void Update(Entity entity)
    {
        progress += Time.deltaTime * rotationSpeedComponent.RotationSpeedWhenCollectLoot;

        var rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
        ownerTransform.Transform.rotation = rotation;

        if (progress >= 1)
        {
            startRotation = Quaternion.identity;
            targetRotation = Quaternion.identity;
            progress = 0;
            EndState();
        }
    }
}
