using System;
using HECSFramework.Core;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Resources, "this system set grade for node")]
    public sealed class SetNodeDifficultySystem : BaseSystem 
    {
        [Required] public NodeDifficultyComponent DifficultyComponent;

        public override void InitSystem()
        {
            var stateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
            var raidManager = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            var difficultyComponent = raidManager.GetComponent<MissionsDifficultyConfigComponent>();
            var missionID = stateComponent.MissionID;
            var mission = raidManager.GetComponent<MissionsConfigsHolderComponent>().GetMissionByID(missionID);

            var difficulty = difficultyComponent.GetDifficultyValue(mission.MissionGrade, mission.MissionType, mission.StartZone);

            DifficultyComponent.NodeDifficulty = difficulty;
        }
    }
}