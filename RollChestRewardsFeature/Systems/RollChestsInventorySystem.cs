using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Inventory, Doc.Loot, "system for RollChestsInventory")]
    public sealed class RollChestsInventorySystem : BaseSystem, IReactCommand<AddChestRewardsCommand>
    {

        [Required] public RollChestsInventoryComponent Inventory;

        public void CommandReact(AddChestRewardsCommand command)
        {
            Inventory.AddChest(command.chestEntity);
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Inventory, Doc.Loot, "by this command we added rewards from roll chest")]
    public struct AddChestRewardsCommand : ICommand
    {
        public Entity chestEntity;
    }
}