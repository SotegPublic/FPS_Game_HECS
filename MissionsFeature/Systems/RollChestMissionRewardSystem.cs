using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;

namespace Systems
{
    [RequiredAtContainer(typeof(MissionRewardTagComponent), typeof(NameComponent))]
    [Serializable][Documentation(Doc.Missions, Doc.Rewards, "this system process mission roll chest rewards")]
    public sealed class RollChestMissionRewardSystem : BaseSystem, IReactCommand<ExecuteMissionRewardCommand>
    {
        [Required] public LootRewardsHolderComponent LocalRewardsHolder;
        [Required] public GradeComponent ChestGrade;

        [Single] public RewardsGlobalHolderComponent RewardsContainersHolder;

        public void CommandReact(ExecuteMissionRewardCommand command)
        {
            ExecuteEquipChestReward();
        }

        private void ExecuteEquipChestReward()
        {
            Owner.Command(new CalculateRollRewardsCommand());

            for (int i = 0; i < LocalRewardsHolder.Rewards.Count; i++)
            {
                var containerIndex = LocalRewardsHolder.Rewards[i];

                if (RewardsContainersHolder.TryGetContainerByID(containerIndex, out var container))
                {
                    var reward = container.GetEntity();

                    reward.Init();

                    reward.Command(new ExecuteChestRewardCommand { GradeID = ChestGrade.Grade });
                    Owner.World.Command(new DestroyEntityWorldCommand { Entity = reward });
                }
            }
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Missions, Doc.Rewards, "ExecuteMissionRewardCommand")]
    public struct ExecuteMissionRewardCommand: ICommand
    {
    }
}