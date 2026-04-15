using System;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Movement, "this system responsible for following the drone to the player")]
    public sealed class LooterDroneFollowSystem : BaseSystem, ILateUpdatable
    {
        [Required] private DroneFollowTargetComponent targetComponent;
        [Required] private DroneFollowVelocityComponent velocityComponent;
        [Required] private DroneFollowConfigComponent configComponent;

        public override void InitSystem()
        {
        }

        public void UpdateLateLocal()
        {
            if (!Owner.ContainsMask<DroneFollowTagComponent>())
                return;

            if (targetComponent.FollowTarget == null) 
                return;

            if (!Owner.World.TryGetEntityBySingleComponent<MainCharacterTagComponent>(out var characterEntity))
                return;

            var isCharacterStoped = characterEntity.ContainsMask<OnStopMovingComponent>();
            var offset = isCharacterStoped ? configComponent.StayPositionOffset : configComponent.MovePositionOffset;
            var speed = isCharacterStoped ? configComponent.ApproachSpeed : configComponent.FollowSpeed;

            var targetPosition = targetComponent.FollowTarget.TransformPoint(offset);

            var transform = Owner.GetTransform();

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityComponent.Velocity, speed);

            var lookDirection = isCharacterStoped ? targetComponent.FollowTarget.forward : targetPosition - transform.position;

            if (lookDirection == Vector3.zero)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, configComponent.FollowRotationSpeed * Time.deltaTime);
        }
    }
}