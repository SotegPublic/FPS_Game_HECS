using System;
using HECSFramework.Core;
using Components;
using UnityEngine;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Player, Doc.Character, Doc.Movement, "Character rotation system")]
    public sealed class PlayerCharacterRotationSystem : BaseSystem, ILateUpdatable
    {
        [Required] public UnityTransformComponent TransformComponent;
        [Required] public MovementComponent MovementComponent;
        [Required] public NavMeshAgentComponent NavMeshAgentComponent;

        public override void InitSystem()
        {
        }

        public void UpdateLateLocal()
        {
            if (Owner.ContainsMask<OnRotatingByRequestTagComponent>())
                return;

            RotateWhileMove();
        }


        private void RotateWhileMove()
        {
            if (Owner.ContainsMask<OnStopMovingComponent>())
                return;

            var zoneStateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();

            var shootingPoint = zoneStateComponent.CurrentShootingPoint;
            var sqrShootingRotateDistanceThreshold = MovementComponent.ShootingRotateDistanceThreshold * MovementComponent.ShootingRotateDistanceThreshold;

            var sqrDistance = (shootingPoint.Entity.GetPosition() - TransformComponent.Transform.position).sqrMagnitude;

            if (sqrDistance > sqrShootingRotateDistanceThreshold)
            {
                Vector3 lookDirection = MovementComponent.MoveDirection;
                lookDirection.y = 0;

                if (lookDirection.magnitude > 0.1f)
                {
                    RotateCharacter(lookDirection);
                }
            }
            else
            {

                var enemiesSpawnPoint = zoneStateComponent.CurrentZone;
                var lookDirection = (enemiesSpawnPoint.Entity.GetPosition() - shootingPoint.Entity.GetPosition()).normalized;
                lookDirection.y = 0;

                RotateCharacter(lookDirection);
            }
        }

        private void RotateCharacter(Vector3 lookDirection)
        {
            var lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            TransformComponent.Transform.rotation =
                Quaternion.RotateTowards(TransformComponent.Transform.rotation, lookRotation, MovementComponent.RotationSpeed * Time.deltaTime);
        }
    }
}