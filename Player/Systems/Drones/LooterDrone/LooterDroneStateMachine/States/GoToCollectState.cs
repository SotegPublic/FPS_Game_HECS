using Components;
using HECSFramework.Core;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GoToCollectState : LooterDroneState
{
    private Vector3 targetPoint;
    private Entity drone;
    private Entity playerCharacter;
    private Vector3 velocity;
    private DroneCollectWhileFollowConfigComponent collectWhileFollowConfig;

    public GoToCollectState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
    }

    public override int StateID => GoToCollectState;

    public override void Enter(Entity entity)
    {
        targetPoint = entity.GetComponent<CollectPointComponent>().Point;
        drone = entity.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
        playerCharacter = entity.World.GetEntityBySingleComponent<MainCharacterTagComponent>();
        collectWhileFollowConfig = drone.GetComponent<DroneCollectWhileFollowConfigComponent>();
    }

    public override void Exit(Entity entity)
    {
    }

    public override void Update(Entity entity)
    {
        var transform = drone.GetTransform();

        Move(transform);

        if (velocity.magnitude <= 1f)
        {
            EndState();
        }
    }

    private void Move(Transform droneTransform)
    {
        droneTransform.position = Vector3.SmoothDamp(droneTransform.position, targetPoint, ref velocity, collectWhileFollowConfig.MoveToCollectTime);
        droneTransform.rotation = Quaternion.LookRotation(playerCharacter.GetTransform().forward);
    }
}
