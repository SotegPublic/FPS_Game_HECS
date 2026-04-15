using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
    [Serializable][Documentation(Doc.Rewards, "loot items reward system")]
    public sealed class ChestEquipItemRewardSystem : BaseSystem, IReactCommand<ExecuteChestRewardCommand>
    {
        [Required] private EquipItemRewardConfigComponent config;
        [Single] public UpgradesGlobalHolderComponent UpgradesGlobalHolder;
        [Single] public EquipItemsHolderComponent EquipItemsHolder;
        [Single] public GradeToUpgradeMapComponent UpgradeMap;
        public void CommandReact(ExecuteChestRewardCommand command)
        {
            if(EquipItemsHolder.TryGetContainerByID(config.BaseItemID, out var baseContainer))
            {
                var targetIndex = UpgradeMap.GetUpgradeIndexByGrade(command.GradeID);
                var targetContainer = baseContainer.GetComponent<UpgradeComponent>().Upgrades[targetIndex];

                Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
                {
                    ItemID = targetContainer.ContainerIndex,
                    Count = 1,
                    InventoryID = InventoryTypeIdentifierMap.ShelterStorageInventory
                });
            }
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Rewards, "ExecuteChestRewardCommand")]
    public struct ExecuteChestRewardCommand : ICommand
    {
        public int GradeID;
    }
}