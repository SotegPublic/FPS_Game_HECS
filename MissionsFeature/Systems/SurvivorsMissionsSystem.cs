using Commands;
using Components;
using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Missions, "this system process and control survivors missions")]
    public sealed class SurvivorsMissionsSystem : BaseSystem, IUpdatable, IReactGlobalCommand<StartSurvivorsMissionCommand>, IReactGlobalCommand<CompleteMissionCommand>
    {
        [Required] public MissionsConfigsHolderComponent MissionConfigsHolder;

        private CurrentMissionsHolderComponent currentMissionsHolder;
        private HashSet<int> activeSurvivorsMissions = new HashSet<int>(8);

        public override void InitSystem()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            currentMissionsHolder = player.GetComponent<CurrentMissionsHolderComponent>();
        }

        public void CommandGlobalReact(StartSurvivorsMissionCommand command)
        {
            var missionConfig = MissionConfigsHolder.GetMissionByID(command.MissionID);
            if (currentMissionsHolder.TryGetActiveMission(command.MissionID, out var activeMission))
            {
                activeMission.EndTimeSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + missionConfig.ConfigForSurvivors.DurationInSeconds;
            }

            Owner.World.Command(new SendSurvivorsOnMissionCommand { SurvivorsCount = missionConfig.ConfigForSurvivors.RequiredSurvivors });

            activeSurvivorsMissions.Add(command.MissionID);
        }

        public void CommandGlobalReact(CompleteMissionCommand command)
        {
            if (!activeSurvivorsMissions.Contains(command.MissionID))
                return;

            if (currentMissionsHolder.TryGetActiveMission(command.MissionID, out var activeMission))
            {
                var missionConfig = MissionConfigsHolder.GetMissionByID(command.MissionID);
                var returnCount = UnityEngine.Random.Range(1, missionConfig.ConfigForSurvivors.RequiredSurvivors + 1);

                Owner.World.Command(new ReturnSurvivorsFromMissionCommand { ReturnCount = returnCount, SendCount = missionConfig.ConfigForSurvivors.RequiredSurvivors });

                activeSurvivorsMissions.Remove(command.MissionID);
            }
        }

        public void UpdateLocal()
        {
            CheckFinishedSurvivorsMissions();
        }

        private void CheckFinishedSurvivorsMissions()
        {
            foreach (var mission in currentMissionsHolder.ActiveMissions)
            {
                if (!mission.IsInProgress)
                    continue;

                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= mission.EndTimeSec)
                    mission.IsFinished = true;
            }
        }
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Missions, "StartSurvivorsMission")]
    public struct StartSurvivorsMissionCommand : IGlobalCommand
    {
        public int MissionID;
    }
}