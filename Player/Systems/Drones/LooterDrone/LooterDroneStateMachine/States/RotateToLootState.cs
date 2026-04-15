using Components;
using HECSFramework.Core;
using UnityEngine;

public class RotateToLootState : LooterDroneState
{
    private UnityTransformComponent ownerTransform;
    private RotationSpeedWhenCollectLootComponent rotationSpeedComponent;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float progress;

    public RotateToLootState(StateMachine stateMachine, int nextDefaultState, UnityTransformComponent transformComponent,
        RotationSpeedWhenCollectLootComponent droneRotationSpeedComponent) : base(stateMachine, nextDefaultState)
    {
        ownerTransform = transformComponent;
        rotationSpeedComponent = droneRotationSpeedComponent;
    }

    public override int StateID => RotateToLootState;

    public override void Enter(Entity entity)
    {
        var targetPosition = entity.GetComponent<CurrentLootTargetComponent>().CurrentCollectLootPosition;
        var dronePosition = entity.GetPosition();
        targetPosition.y = dronePosition.y;

        var direction = (targetPosition - dronePosition).normalized;

        startRotation = entity.GetRotation();
        targetRotation = Quaternion.LookRotation(direction, Vector3.up);
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
