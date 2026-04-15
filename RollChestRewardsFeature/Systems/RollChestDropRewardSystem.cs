using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using Random = UnityEngine.Random;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Rewards, "drop roll chest reward system")]
    public sealed class RollChestDropRewardSystem : BaseSystem, IReactCommand<ExecuteReward>
    {
        [Required] public RollChestDropRewardConfigComponent Config;
        public void CommandReact(ExecuteReward command)
        {
            var amount = Config.IsCountBetweenValues ? Random.Range(Config.MinRewardCount, Config.MaxRewardCount + 1) : Config.RewardCount;

            Owner.Command(new CalculateRollRewardsCommand());
            Owner.World.Command(new GlobalItemRewardCommand
            {
                Amount = amount,
                From = new AliveEntity(Owner),
                To = new AliveEntity(command.Target),
                RewardView = Owner.AsActor().GameObject,
                DrawRule = Config.DrawRuleID,
                RewardItemID = Owner.GetContainerIndex()
            });
        }

        public override void InitSystem()
        {
        }
    }
}