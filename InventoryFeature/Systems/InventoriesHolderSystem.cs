using System;
using HECSFramework.Core;
using Components;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Inventory, "InventoriesHolderSystem")]
    public sealed class InventoriesHolderSystem : BaseSystem
    {
        [Required] private InventoryConfigsHolderComponent configsHolder;
        [Required] private InfiniteInventoryConfigsHolderComponent infiniteConfigsHolder;
        [Required] private InventoriesHolderComponent inventoriesHolder;

        public override void InitSystem()
        {
            foreach (var c in Owner.GetComponentsByType<IInventoryComponent>())
            {
                inventoriesHolder.AddInventory(c);
            }

            for (int i = 0; i < configsHolder.InventoryConfigs.Length; i++)
            {
                if (inventoriesHolder.TryGetInventoryByID(configsHolder.InventoryConfigs[i].InventoryTypeID, out var inventory))
                {
                    inventory.CreateInventory(configsHolder.InventoryConfigs[i].SlotsCount);
                }
            }

            for (int i = 0; i < infiniteConfigsHolder.InventoryConfigs.Length; i++)
            {
                if (inventoriesHolder.TryGetInventoryByID(infiniteConfigsHolder.InventoryConfigs[i].InventoryTypeID, out var inventory))
                {
                    inventory.CreateInventory
                    (
                        infiniteConfigsHolder.InventoryConfigs[i].SlotCount, 
                        true,                       
                        infiniteConfigsHolder.InventoryConfigs[i].Multiplicity
                    );
                }
            }
        }
    }
}