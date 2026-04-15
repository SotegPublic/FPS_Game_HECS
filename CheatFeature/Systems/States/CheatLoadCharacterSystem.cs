using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Linq;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Character, Doc.Cheats, "this system spawn character")]
    public sealed class CheatLoadCharacterSystem : BaseGameStateSystem, IUpdatable
    {
        [Required] private CheatStartParametersComponent cheatStartParameters;
        [Single] public ShooterZoneStateComponent ZoneState;

        protected override int State { get; } = GameStateIdentifierMap.CheatLoadPlayerCharacter;

        private EntitiesFilter zonesFilter;

        public override void InitSystem()
        {
            zonesFilter = Owner.World.GetFilter<ZoneTagComponent>();
        }

        protected async override void ProcessState(int from, int to)
        {
            AddItems();

            var mission = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>()
                .GetComponent<MissionsConfigsHolderComponent>().GetMissionByID(cheatStartParameters.MissionID);
            var enterPoint = ZoneState.EnterPoint.Entity;
            var enterPointIndex = enterPoint.GetComponent<ZoneIndexComponent>().Index;
            var targetPoint = enterPointIndex == cheatStartParameters.TargetZoneIndex ? enterPoint : GetTargetShootingPositionAndComplitedZones(mission);



            var point = targetPoint.GetPosition();
            var rotation = targetPoint.GetRotation();

            if (!targetPoint.ContainsMask<EnterZoneTagComponent>())
            {
                cheatStartParameters.StartShootingPoint = targetPoint;

                var zones = Owner.World.GetFilter<ShooterZoneTagComponent>();
                zones.ForceUpdateFilter();
                var targetZone = zones.FirstOrDefault(x => x.GetComponent<ZoneIndexComponent>().Index == cheatStartParameters.TargetZoneIndex).GetAliveEntity();

                var direction = targetZone.Entity.GetPosition() - point;
                var normalDirection = direction.normalized;

                point = point - normalDirection * 10;
                rotation = Quaternion.LookRotation(direction, Vector3.up);
            }


            var mainCharacter = await Owner.World.Request<UniTask<Actor>, SpawnMainCharacterCommand>(new SpawnMainCharacterCommand { Point = point, Rotation = rotation });
            mainCharacter.Init();
        }

        private Entity GetTargetShootingPositionAndComplitedZones(MissionConfig mission)
        {
            zonesFilter.ForceUpdateFilter();
            var targetZone = zonesFilter.FirstOrDefault(z => z.GetComponent<ZoneIndexComponent>().Index == cheatStartParameters.TargetZoneIndex);

            var index = 0;
            var path = mission.PathNodes;

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != cheatStartParameters.TargetZoneIndex)
                {
                    ZoneState.CompleteZones.Add(path[i]);
                    index++;
                }
                else
                    break;
            }

            var previouseZoneID = path[index - 1];

            var shootingZones = targetZone.GetComponent<ShootingZonesHolderComponent>().ShootingPositions;
            var targetShootingZone = shootingZones.Where(sp => sp.Entity.GetComponent<ZoneLinksComponent>().LinkedZones
                                    .Any(linkedZoneID => linkedZoneID == previouseZoneID))
                                .FirstOrDefault().Entity;

            return targetShootingZone;
        }

        private Entity TryGetShootingPointBeforeFinishNode(PathNode startPosition, int previousZoneID, RaidPathTreeHolderComponent raidTreeHolder)
        {
            foreach(var nextNodeID in startPosition.AvailablePathIDs)
            {
                if(nextNodeID == previousZoneID) 
                    continue;

                if (nextNodeID != cheatStartParameters.TargetZoneIndex)
                {
                    var nextNode = raidTreeHolder.PathTree[nextNodeID];
                    var nextStepResult = TryGetShootingPointBeforeFinishNode(nextNode, startPosition.ZoneID, raidTreeHolder);

                    if (nextStepResult != null)
                    {
                        ZoneState.CompleteZones.Add(startPosition.ZoneID);
                        return nextStepResult;
                    }
                }
                else
                {
                    ZoneState.CompleteZones.Add(startPosition.ZoneID);
                    var filter = Owner.World.GetFilter(Filter.Get<ZoneIndexComponent>());
                    foreach(var zone in filter)
                    {
                        if(zone.GetComponent<ZoneIndexComponent>().Index == nextNodeID)
                            return zone.GetComponent<ShootingZonesHolderComponent>()
                                .ShootingPositions
                                .Where(shootingPointActor => shootingPointActor.Entity.GetComponent<ZoneLinksComponent>().LinkedZones
                                    .Any(linkedZoneID => linkedZoneID == startPosition.ZoneID))
                                .FirstOrDefault().Entity;
                    }
                }
            }

            return null;
        }

        private void AddItems()
        {
            Owner.World.Command(new ClearInventoryCommand { InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft });
            Owner.World.Command(new ClearInventoryCommand { InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight });
            Owner.World.Command(new ClearInventoryCommand { InventoryID = InventoryTypeIdentifierMap.PlayerArtefactInventory });

            var addedWeaponsCount = 0;

            foreach (var weapon in cheatStartParameters.Weapons)
            {
                var targetInventory = addedWeaponsCount <= 2 ? InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft : InventoryTypeIdentifierMap.PlayerWeaponInventoryRight;

                Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
                {
                    ItemID = weapon.ContainerIndex,
                    Count = 1,
                    InventoryID = targetInventory
                });

                addedWeaponsCount++;
            }

            foreach (var artefact in cheatStartParameters.Artefacts)
            {
                Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
                {
                    ItemID = artefact.ContainerIndex,
                    Count = 1,
                    InventoryID = InventoryTypeIdentifierMap.PlayerArtefactInventory
                });
            }
        }

        public void UpdateLocal()
        {
            if (!Owner.World.IsHaveSingleComponent<MainCharacterTagComponent>())
                return;
            if (!Owner.World.IsHaveSingleComponent<ShieldTagComponent>())
                return;

            EndState();
        }
    }
}