using System;
using HECSFramework.Core;
using Components;
using Commands;
using System.Collections.Generic;

namespace Systems
{
	[Serializable][Documentation(Doc.Inventory, DocFeature.InventoryFeature, "this system process manual move items by drag")]
    public sealed class ManualMoveInventorySystem : BaseInventorySystem
    {
        private InventoryItem tmpNewItem = new InventoryItem();
        private InventoryItem tmpOldItem = new InventoryItem();

        public override void InitSystem()
        {
        }

        public ManualMoveItemRequestResult ManualMove(ManualMoveItemRequestCommand command)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

            var toInventory = inventoriesHolder.GetInventoryByID(command.ToInventory);
            var fromInventory = inventoriesHolder.GetInventoryByID(command.FromInventory);

            var item = fromInventory.GetItem(command.FromSlot);

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                var itemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType;

                if (inventoriesMap.IsInventoryCanContainsType(command.ToInventory, itemType))
                {
                    var isStackable = itemContainer.TryGetComponent<StackableItemTagComponent>(out var stackableComponent);

                    if (toInventory.TryGetItem(command.ToSlot, out var inventoryItem))
                    {
                        if (isStackable && inventoryItem.ContainerIndex == item.ContainerIndex)
                        {
                            return StackItems(command, toInventory, fromInventory, item.Count);
                        }
                        else
                        {
                            return SwapItems(command, fromInventory, toInventory);
                        }
                    }
                    else
                    {
                        return MoveItem(command, item);
                    }
                }
            }

            return new ManualMoveItemRequestResult
            {
                IsAddSuccess = false
            };
        }

        public ManualMoveAllItemsRequestResult ManualMove(ManualMoveAllItemsRequestCommand command)
        {
            var toInventory = inventoriesHolder.GetInventoryByID(command.ToInventory);
            var fromInventory = inventoriesHolder.GetInventoryByID(command.FromInventory);

            var moveItemcommand = new ManualMoveItemRequestCommand
            {
                ToInventory = command.ToInventory,
                FromInventory = command.FromInventory,
            };

            using var items = fromInventory.GetItems();

            for (int i = 0; i < items.Count; i++) 
            {
                var value = items.Items[i].Value;

                if (value.IsAvailable)
                    continue;
                
                toInventory.TryGetFreeSlot(value.ContainerIndex, out moveItemcommand.ToSlot);
                moveItemcommand.FromSlot = i;

                ManualMove(moveItemcommand);
            }

            return new ManualMoveAllItemsRequestResult
            {
                IsAddSuccess = false
            };
        }

        private ManualMoveItemRequestResult MoveItem(ManualMoveItemRequestCommand command, InventoryItem item)
        {
            if (command.ToInventory == command.FromInventory)
            {
                MoveItemToAnotherSlot(command.ToInventory, command.FromSlot, command.ToSlot);
            }
            else
            {
                MoveToNewInventory(command, item);
            }

            return new ManualMoveItemRequestResult
            {
                IsAddSuccess = true
            };
        }

        private void MoveToNewInventory(ManualMoveItemRequestCommand command, InventoryItem item)
        {
            tmpNewItem.Copy(item);

            RemoveItemFromSlot(command.FromSlot, command.FromInventory);
            AddItemToSlot(tmpNewItem, command.ToSlot, command.ToInventory);

            tmpNewItem.CleanData();
        }

        private ManualMoveItemRequestResult SwapItems(ManualMoveItemRequestCommand command, IInventoryComponent fromInventory, IInventoryComponent toInventory)
        {
            var swapedItem = toInventory.GetItem(command.ToSlot);
            var newItem = fromInventory.GetItem(command.FromSlot);

            if (command.ToInventory == command.FromInventory)
            {
                SwapItemsInInventory(command.ToInventory, command.FromSlot, command.ToSlot);
            }
            else
            {
                SwapBetweenInventories(command, newItem, swapedItem);
            }

            return new ManualMoveItemRequestResult
            {
                IsAddSuccess = true
            };
        }

        private ManualMoveItemRequestResult StackItems(ManualMoveItemRequestCommand command, IInventoryComponent toInventory, IInventoryComponent fromInventory, int addedCount)
        {
            var newCount = toInventory.GetItem(command.ToSlot).Count + addedCount;
            ChangeItemCount(toInventory.InventoryID, command.ToSlot, newCount);
            RemoveItemFromSlot(command.FromSlot, fromInventory.InventoryID);

            return new ManualMoveItemRequestResult
            {
                IsAddSuccess = true
            };
        }

        private void SwapBetweenInventories(ManualMoveItemRequestCommand command, InventoryItem newItem, InventoryItem returnedItem)
        {
            tmpOldItem.Copy(returnedItem);
            tmpNewItem.Copy(newItem);

            RemoveItemFromSlot(command.ToSlot, command.ToInventory, false);
            RemoveItemFromSlot(command.FromSlot, command.FromInventory, false);

            AddItemToSlot(tmpNewItem, command.ToSlot, command.ToInventory, false);
            AddItemToSlot(tmpOldItem, command.FromSlot, command.FromInventory, false);

            Owner.World.Command
            (
                new OnSwapItemsCommand
                {
                    FromInventoryID = command.FromInventory,
                    FromSlot = command.FromSlot,
                    FromItem = new InventoryItem(tmpOldItem),
                    
                    ToInventoryID = command.ToInventory,
                    ToSlot = command.ToSlot,
                    ToItem = new InventoryItem(tmpNewItem),
                }
            );

            tmpNewItem.CleanData();
            tmpOldItem.CleanData();
        }
    }
}