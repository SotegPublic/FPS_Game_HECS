using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Loot, Doc.Rewards, "this system calculate rewards form chest roll")]
    public sealed class CalculateChestRewardsSystem : BaseCalculateRewardsSystem, IReactCommand<CalculateRollRewardsCommand>
    {
        [Required] public ChestRewardsConfigComponent DropConfigComponent;

        public override void InitSystem()
        {
        }

        public void CommandReact(CalculateRollRewardsCommand command)
        {
            DropConfigComponent.InitConfig();
            CalculateAndAddRandomRewards(DropConfigComponent.ChestRewardsConfig);

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
    [Documentation(Doc.Loot, Doc.Rewards, "by this command we calculate chest roll rewards")]
    public struct CalculateRollRewardsCommand : ICommand
    {
    }
}
