using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.Shelter, "here we load player shelter")]
    public sealed class LoadPlayerShelterSystem : BaseGameStateSystem
    {
        [Required] private ShelterConfigComponent shelterConfig;
        [Required] private ShelterRoomContainersHolderComponent roomsHolder;

        private List<UniTask> spawnRoomtaskList;

        protected override int State => GameStateIdentifierMap.LoadPlayerShelterState;

        public override void InitSystem()
        {
            spawnRoomtaskList = new List<UniTask>(shelterConfig.RoomsHorizontalCount * shelterConfig.RoomsVerticalCount);
        }

        protected override void ProcessState(int from, int to)
        {
            LoadRoomsAsync().Forget();
        }

        private async UniTask LoadRoomsAsync()
        {
            var shelterRoomsComponent = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ShelterRoomsComponent>();
            var parent = Owner.World.GetEntityBySingleComponent<RoomsParentTagComponent>().GetTransform();

            for (int i = 0; i < shelterRoomsComponent.RoomsArray.Length; i++)
            {
                spawnRoomtaskList.Add(SpawnRoomAsync(shelterRoomsComponent.RoomsArray[i], parent));
            }

            await UniTask.WhenAll(spawnRoomtaskList);

            LoadSurvivorsCount();

            var progress = Owner.World.GetSingleComponent<ShooterPlayerProgressComponent>();
            if(progress.CurrentScenarioIndex == 0)
            {
                var shooterZoneStateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
                var raidManager = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
                var missions = raidManager.GetComponent<MissionsConfigsHolderComponent>();

                var currentScenarioMissionID = missions.GetScenarioMissionID(progress.CurrentScenarioIndex);
                var mission = missions.GetMissionByID(currentScenarioMissionID);

                shooterZoneStateComponent.EnterID = mission.StartZone;
                shooterZoneStateComponent.MissionID = mission.MissionId;

                Owner.World.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.LoadZone });
            }
            else
            {
                EndState();
            }
        }

        private void LoadSurvivorsCount()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var countersHolder = player.GetComponent<CountersHolderComponent>();

            if(countersHolder.TryGetIntCounter(CounterIdentifierContainerMap.SurvivorsStorage, out var storageCounter))
            {
                if(countersHolder.TryGetIntCounter(CounterIdentifierContainerMap.Survivors, out var survivorsCounter))
                {
                    survivorsCounter.SetValue(storageCounter.Value);
                }
            }
        }

        protected override async void OnExitState()
        {
            base.OnExitState();
            Owner.World.Command(new ShelterDataWasLoadedCommand());
        }

        private async UniTask SpawnRoomAsync(RoomInfo roomInfo, Transform parent)
        {
            if (roomsHolder.TryGetContainerByRoomIdentifier(roomInfo.RoomID, out var roomContainer))
            {
                var roomActor = await roomContainer.GetActor(position: parent.position, transform: parent);
                roomActor.Init();

                await new WaitFor<ViewReadyTagComponent>(roomActor.Entity).RunJob();
                await UniTask.DelayFrame(1);

                roomActor.Entity.GetComponent<RoomTagComponent>().SetRoomGridIndex(roomInfo.RoomMatrixIndex);
            }
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shelter, "ShelterWasLoadedCommand")]
    public struct ShelterDataWasLoadedCommand : IGlobalCommand { }
}