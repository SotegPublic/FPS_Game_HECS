using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Player, "this system is responsible for rescue")]
    public sealed class DroneRescueSystem : BaseSystem, IReactCommand<ActivateRescueCommand>, IReactCommand<MoveByCurveEndedCommand>
    {
        [Required] private RescueDroneConfigComponent config;


        public void CommandReact(ActivateRescueCommand command)
        {
            var playerCharacterView = Owner.World.GetEntityBySingleComponent<PlayerCharacterComponent>().GetComponent<ViewReadyTagComponent>().View;

            var moveTag = Owner.GetOrAddComponent<MoveByCurveToV3TagComponent>();
            moveTag.From = Owner.GetPosition();
            moveTag.To = playerCharacterView.transform.position + Vector3.up * config.DroneUpOffset;
            moveTag.DrawRule = MoveByCurveDrawRuleIdentifierMap.DroneCurveIdentifier;
        }

        public async void CommandReact(MoveByCurveEndedCommand command)
        {
            if (Owner.ContainsMask<RescueDoneTagComponent>())
            {
                Owner.RemoveComponent<RescueDoneTagComponent>();

                var mainChar = Owner.World.GetEntityBySingleComponent<MainCharacterTagComponent>();
                mainChar.AddComponent<PlayerCharacterRescuedTagComponent>();

                return;
            }

            var playerCharacterView = Owner.World.GetEntityBySingleComponent<PlayerCharacterComponent>().GetComponent<ViewReadyTagComponent>().View;
            var ownerTransform = Owner.GetTransform();

            playerCharacterView.transform.SetParent(ownerTransform, true);

            await UniTask.Delay(config.WaitTimeMilliseconds);

            var moveTag = Owner.GetOrAddComponent<MoveByCurveToV3TagComponent>();
            moveTag.From = Owner.GetPosition();
            moveTag.To = Owner.GetPosition()  + config.ReturnDirection * config.DroneReturnBackDistance + Vector3.up * config.DroneReturnBackDistance;
            moveTag.DrawRule = MoveByCurveDrawRuleIdentifierMap.DroneCurveIdentifier;
            Owner.AddComponent<RescueDoneTagComponent>();
        }

        public override void InitSystem()
        {
        }
    }
}