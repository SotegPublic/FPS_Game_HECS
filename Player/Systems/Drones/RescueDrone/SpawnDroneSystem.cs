using System;
using HECSFramework.Core;
using Components;
using Commands;
using UnityEngine;
using HECSFramework.Unity;

namespace Systems
{
	[Serializable][Documentation(Doc.Death, "this system spawn rescue drone")]
    public sealed class SpawnDroneSystem : BaseSystem, IReactCommand<IsDeadCommand>
    {
        [Required] private DroneContainerHolderComponent containerHolderComponent;
        [Required] private RescueDroneSpawnConfigComponent spawnConfig;


        public async void CommandReact(IsDeadCommand command)
        {
            var zoneStateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
            var shootingPosition = zoneStateComponent.CurrentShootingPoint.Entity.GetPosition();
            var enemyPosition = zoneStateComponent.CurrentZone.Entity.GetPosition();

            var direction = (enemyPosition - shootingPosition).normalized;
            var backOffset = direction * spawnConfig.DroneSpawnBackOffset;
            var upOffset = Vector3.up * spawnConfig.DroneSpawnHieghtOffset;

            var newPosition = shootingPosition - backOffset + upOffset;

            var droneActor = await containerHolderComponent.DroneContainer.GetActor(position: newPosition);
            droneActor.Init();

            await new WaitFor<ViewReadyTagComponent>(droneActor.Entity).RunJob();

            droneActor.Entity.GetComponent<RescueDroneConfigComponent>().ReturnDirection = direction * -1;

            droneActor.Entity.Command(new ActivateRescueCommand());
        }

        public override void InitSystem()
        {
        }
    }
}