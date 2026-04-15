using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Loot, Doc.Rewards, "this system calculate rewards form drop config")]
    public sealed class CalculateRewardsSystem : BaseCalculateRewardsSystem, IReactCommand<CalculateRewardsCommand>
    {
        [Required] public LootDropConfigsComponent DropConfigs;

        public override void InitSystem()
        {
        }

        public void CommandReact(CalculateRewardsCommand command)
        {
            DropConfigs.InitConfigs();

            if (DropConfigs.TryGetDropConfigByGrade(command.GradeID, out var config))
            {
                for (int i = 0; i < config.GuaranteedRewards.Length; i++)
                {
                    RewardsHolder.Rewards.Add(config.GuaranteedRewards[i]);
                }

                CalculateAndAddRandomRewards(config);
            }

            Owner.AddComponent<LootCalculatedTagComponent>();
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            Owner.RemoveComponent<LootCalculatedTagComponent>();
        }
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Loot, Doc.Rewards, "by this command we calculate drop rewards")]
    public struct CalculateRewardsCommand : ICommand
    {
        public int GradeID;
    }
}