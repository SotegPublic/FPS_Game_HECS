using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Missions, "this system completes mission")]
    public sealed class MissionsCompletionSystem : BaseSystem, IGlobalStart, IReactGlobalCommand<CompleteMissionCommand>
    {
        [Required] public MissionsConfigsHolderComponent MissionConfigsHolder;

        private RewardsGlobalHolderComponent rewardsHolder;
        private CurrentMissionsHolderComponent currentMissionsHolder;

        public void CommandGlobalReact(CompleteMissionCommand command)
        {
            if (currentMissionsHolder.TryGetActiveMission(command.MissionID, out var activeMission))
            {
                activeMission.IsCompleted = true;
                var missionConfig = MissionConfigsHolder.GetMissionByID(command.MissionID);
                
                ProcessRewards(missionConfig);
            }
        }

        private void ProcessRewards(MissionConfig missionConfig)
        {
            if (rewardsHolder.TryGetContainerByID(missionConfig.RewardID, out var reward))
            {
                var chestEntity = reward.GetEntity();
                chestEntity.Init();

                chestEntity.Command(new ExecuteMissionRewardCommand());
                Owner.World.Command(new DestroyEntityWorldCommand { Entity = chestEntity });
            }
        }

        public override void InitSystem()
        {
        }

        public void GlobalStart()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            currentMissionsHolder = player.GetComponent<CurrentMissionsHolderComponent>();
            rewardsHolder = Owner.World.GetEntityBySingleComponent<GlobalRewarsFeatureTagComponent>().GetComponent<RewardsGlobalHolderComponent>();
        }
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Missions, "CompleteMissionCommand")]
    public struct CompleteMissionCommand : IGlobalCommand
    {
        public int MissionID;
    }
}