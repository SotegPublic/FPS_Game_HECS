using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Inventory, DocFeature.InventoryFeature, "this system process auto move items by click")]
    public sealed class AutoMoveInventorySystem : BaseInventorySystem
    {
        [Required] private AutoMoveByClickUIRulesComponent autoMoveRules;

        public override void InitSystem()
        {
        }

        public AutoMoveItemRequestResult AutoMove(AutoMoveItemRequestCommand command)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();
            var inventoriesHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<InventoriesHolderComponent>();

            var fromInventory = inventoriesHolder.GetInventoryByID(command.FromInventory);
            var item = fromInventory.GetItem(command.FromSlot);

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                var itemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType;
                var isStackable = itemContainer.TryGetComponent<StackableItemTagComponent>(out var _);

                var inventoriesIDs = autoMoveRules.GetInventortiesIDs(command.UIWindowID, command.FromInventory);                
                if (inventoriesIDs != null)
                {
                    for (int i = 0; i < inventoriesIDs.Length; i++)
                    {
                        if (!inventoriesHolder.TryGetInventoryByID(inventoriesIDs[i], out var toInventory))
                            continue;

                        if (!inventoriesMap.IsInventoryCanContainsType(inventoriesIDs[i], itemType))
                            continue;

                        if (inventoriesIDs[i] == command.FromInventory)
                            continue;

                        if (isStackable && toInventory.TryGetSlotWithSameItem(item.ContainerIndex, out var slot))
                        {
                            if (!command.InfoOnly)
                            {
                                UpdateCountInItems(command, slot, toInventory, fromInventory, item.Count);
                            }

                            return new AutoMoveItemRequestResult
                            {
                                IsAddSuccess = true,
                                NewSlot = slot,
                                NewInventoryID = toInventory.InventoryID
                            };
                        }
                        else
                        {
                            if (!toInventory.TryGetFreeSlot(item.ContainerIndex, out var freeSlot))
                                continue;

                            if (!command.InfoOnly)
                            {
                                AddNewItem(command, toInventory.InventoryID, freeSlot, item);
                            }

                            return new AutoMoveItemRequestResult
                            {
                                IsAddSuccess = true,
                                NewSlot = freeSlot,
                                NewInventoryID = toInventory.InventoryID
                            };
                        }
                    }
                }
            }

            return new AutoMoveItemRequestResult
            {
                IsAddSuccess = false,
                NewSlot = default,
                NewInventoryID = default
            };
        }

        private void UpdateCountInItems(AutoMoveItemRequestCommand command, int toSlot, IInventoryComponent toInventory, IInventoryComponent fromInventory, int addedCount)
        {
            var newCount = toInventory.GetItem(toSlot).Count + addedCount;

            ChangeItemCount(toInventory.InventoryID, toSlot, newCount);
            RemoveItemFromSlot(command.FromSlot, fromInventory.InventoryID);
        }

        private void AddNewItem(AutoMoveItemRequestCommand command, int to, int toSlot, InventoryItem item)
        {
            AddItemToSlot(item, toSlot, to);
            RemoveItemFromSlot(command.FromSlot, command.FromInventory);
        }
    }
}