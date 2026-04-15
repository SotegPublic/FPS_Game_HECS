using Commands;
using Components;
using HECSFramework.Core;
using Sirenix.Utilities;
using System;
using System.Linq;

namespace Systems
{
    [Serializable][Documentation(Doc.Missions, Doc.Shelter, "this system generate missions list for shelter")]
    public sealed class MissionsListSystem : BaseSystem, IReactGlobalCommand<UpdateMissionsListCommand>, IUpdatable
    {
        [Single] public ShooterPlayerProgressComponent ProgressComponent;
        [Required] public MissionsConfigsHolderComponent ConfigsHolder;

        private CurrentMissionsHolderComponent currentMissionsHolder;
        private OpenedEntersComponent openedEntersComponent;
        private HECSList<int> listForMissionsID = new HECSList<int>(32);

        public override void InitSystem()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            currentMissionsHolder = player.GetComponent<CurrentMissionsHolderComponent>();
            openedEntersComponent = player.GetComponent<OpenedEntersComponent>();
        }
        public void UpdateLocal()
        {
            if (ProgressComponent.IsInScenario)
                return;

            var currentTimeSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var updateTime = currentMissionsHolder.LastMissionsUpdateTime + currentMissionsHolder.UpdateListFrequencyInSec;

            if (currentTimeSec >= updateTime)
            {
                UpdateMissionsList();
            }
        }

        public void CommandGlobalReact(UpdateMissionsListCommand command)
        {
            UpdateMissionsList();
        }

        private void UpdateMissionsList()
        {
            if (ProgressComponent.IsInScenario)
            {
                var currentScenarioMissionID = ConfigsHolder.GetScenarioMissionID(ProgressComponent.CurrentScenarioIndex);
                var mission = ConfigsHolder.GetMissionByID(currentScenarioMissionID);

                currentMissionsHolder.ResetComplitedAndNotProgressedMissions();
                currentMissionsHolder.ActiveMissions[0].MissionID = mission.MissionId;
            }
            else
            {
                currentMissionsHolder.ResetComplitedAndNotProgressedMissions();
                var activeMissionsCount = currentMissionsHolder.GetActiveMissionsCount();
                if (activeMissionsCount == currentMissionsHolder.MissionsListLength)
                    return;

                var neededCount = currentMissionsHolder.MissionsListLength - activeMissionsCount;

                for (int i = 0; i < ConfigsHolder.Missions.Length; i++)
                {
                    var mission = ConfigsHolder.Missions[i];
                    if (!openedEntersComponent.OpenedEntersIDs.Contains(mission.StartZone))
                        continue;
                    if (mission.IsScenarioMission)
                        continue;
                    if (currentMissionsHolder.TryGetActiveMission(mission.MissionId, out var activeMission))
                        continue;

                    listForMissionsID.Add(mission.MissionId);
                }

                for (int i = 0; i < neededCount; i++)
                {
                    if (listForMissionsID.Count == 0)
                        break;

                    var missionID = listForMissionsID[UnityEngine.Random.Range(0, listForMissionsID.Count)];
                    currentMissionsHolder.ActiveMissions[i].MissionID = missionID;
                    listForMissionsID.RemoveSwap(missionID);
                }

                currentMissionsHolder.SortAfterAddedNewMissions();
            }

            currentMissionsHolder.LastMissionsUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Owner.World.Command(new MissionsListRedrawCommand());
        }
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Missions, Doc.Shelter, "command for update current missions list")]
    public struct UpdateMissionsListCommand : IGlobalCommand { }

    [Serializable]
    [Documentation(Doc.Missions, Doc.Shelter, "we send this command when missions list was updated")]
    public struct MissionsListRedrawCommand : IGlobalCommand { }
}