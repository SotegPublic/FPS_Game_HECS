using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Player, Doc.Movement, "this system rotate character by requests")]
    public sealed class PlayerCharacterManualRotationSystem : BaseSystem, ILateUpdatable, IRequestProvider<UniTask, ManualRotateCommand>
    {
        [Required] private UnityTransformComponent transformComponent;
        [Required] private MovementComponent movementComponent;
        [Required] private ManualRotationStateComponent stateComponent;

        public override void InitSystem()
        {
        }


        public async UniTask Request(ManualRotateCommand command)
        {
            if (command.OwnerIndex != Owner.Index)
                return;

            stateComponent.StartRotation = transformComponent.Transform.rotation;

            stateComponent.EndRotation = command.TargetRotation;

            if (Quaternion.Angle(stateComponent.StartRotation, stateComponent.EndRotation) < movementComponent.MinAllowedRotationAngle)
                return;

            Owner.AddComponent<OnRotatingByRequestTagComponent>();

            await new WaitRemove<OnRotatingByRequestTagComponent>(Owner).RunJob();
        }

        public void UpdateLateLocal()
        {
            if (!Owner.ContainsMask<OnRotatingByRequestTagComponent>())
                return;

            RotateByRequest();
        }

        private void RotateByRequest()
        {
            var newRotation = Quaternion.RotateTowards(transformComponent.Transform.rotation, stateComponent.EndRotation, Time.deltaTime * movementComponent.RotationSpeed);
            transformComponent.Transform.rotation = newRotation;

            var angle = Quaternion.Angle(transformComponent.Transform.rotation, stateComponent.EndRotation);

            //Debug.Log((Time.deltaTime * movementComponent.RotationSpeed).ToString());

            if (angle <= movementComponent.MinAllowedRotationAngle)
            {
                Owner.RemoveComponent<OnRotatingByRequestTagComponent>();
            }
        }
    }
}