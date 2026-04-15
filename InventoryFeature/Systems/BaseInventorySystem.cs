using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Inventory, DocFeature.InventoryFeature, "base inventory system")]
    public abstract class BaseInventorySystem : BaseSystem 
    {
        [Required] protected InventoryMappingConfigComonent inventoriesMap;
        [Required] protected InventoriesHolderComponent inventoriesHolder;

        public override void InitSystem()
        {
        }

        protected void SwapItemsInInventory(int inventoryID, int fromSlot, int toSlot)
        {
            if (inventoriesHolder.TryGetInventoryByID(inventoryID, out var inventory))
            {
                inventory.SwapItems(fromSlot, toSlot);
            }
        }

        protected void AddItemToSlot(InventoryItem item, int toSlot, int inventoryID, bool withCommand = true)
        {
            if (inventoriesHolder.TryGetInventoryByID(inventoryID, out var inventory))
            {
                inventory.AddItem(toSlot, item);
                if (withCommand)
                {
                    Owner.World.Command(new OnAddItemToInventoryCommand() { InventoryID = inventoryID, Slot = toSlot, Item = new InventoryItem(item) });
                }
            }
        }

        protected void RemoveItemFromSlot(int slot, int inventoryID, bool withCommand = true)
        {
            if (inventoriesHolder.TryGetInventoryByID(inventoryID, out var inventory))
            {
                var item = withCommand ? new InventoryItem(inventory.GetItem(slot)) : null;
                inventory.RemoveItem(slot);
                if (withCommand)
                {
                    Owner.World.Command(new OnRemoveItemFromInventoryCommand() { InventoryID = inventoryID, Slot = slot, Item = item });
                }
            }
        }

        protected void ChangeItemCount(int inventoryID, int slot, int newCount)
        {
            if (inventoriesHolder.TryGetInventoryByID(inventoryID, out var inventory))
            {
                inventory.GetItem(slot).Count = newCount;
            }
        }

        protected void MoveItemToAnotherSlot(int inventoryID, int fromSlot, int toSlot)
        {
            if (inventoriesHolder.TryGetInventoryByID(inventoryID, out var inventory))
            {
                inventory.MoveItem(toSlot, fromSlot);
            }
        }
    }
}

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.UI, "OnAddItemToInventoryCommand")]
    public struct OnAddItemToInventoryCommand : IGlobalCommand
    {
        public int InventoryID;
        public int Slot; 
        public InventoryItem Item;
    }

    [Documentation(Doc.Inventory, Doc.UI, "OnRemoveItemFromInventoryCommand")]
    public struct OnRemoveItemFromInventoryCommand : IGlobalCommand
    {
        public int InventoryID;
        public int Slot;
        public InventoryItem Item;
    }

    [Documentation(Doc.Inventory, Doc.UI, "OnSwapItemsCommand")]
    public struct OnSwapItemsCommand : IGlobalCommand
    {
        public int FromInventoryID;
        public int FromSlot;
        public InventoryItem FromItem;

        public int ToInventoryID;
        public int ToSlot;
        public InventoryItem ToItem;
    }
}