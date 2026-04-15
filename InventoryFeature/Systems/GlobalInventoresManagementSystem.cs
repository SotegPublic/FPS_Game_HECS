using HECSFramework.Core;
using Components;
using Commands;
using System;
using HECSFramework.Unity;

namespace Systems
{
    [Documentation(Doc.Inventory, DocFeature.InventoryFeature, "this system globally operates inventories")]
    public sealed class GlobalInventoresManagementSystem : BaseSystem, 
        IRequestProvider<ManualMoveItemRequestResult, ManualMoveItemRequestCommand>,
        IRequestProvider<ManualMoveAllItemsRequestResult, ManualMoveAllItemsRequestCommand>,
        IRequestProvider<AutoMoveItemRequestResult, AutoMoveItemRequestCommand>, 
        IRequestProvider<RemoveItemRequestResult, RemoveItemRequestCommand>,
        IRequestProvider<AddItemRequestResult, AddItemRequestCommand>, IReactGlobalCommand<ClearInventoryCommand>
    {
        [Required] private InventoriesHolderComponent inventoriesHolder;
        [Required] private InventoryMappingConfigComonent inventoryMappingConfig;

        [Single] private AutoMoveInventorySystem autoMoveSystem;
        [Single] private ManualMoveInventorySystem manualMoveSystem;

        public override void InitSystem()
        {
        }

        public void CommandGlobalReact(ClearInventoryCommand command)
        {
            var targetInventory = inventoriesHolder.GetInventoryByID(command.InventoryID);
            using var items = targetInventory.GetItems();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items.Items[i];
                if (!item.Value.IsAvailable)
                    targetInventory.RemoveItem(item.Key);
            }
        }

        #region AutoMoveRequest
        public AutoMoveItemRequestResult Request(AutoMoveItemRequestCommand command)
        {
            return autoMoveSystem.AutoMove(command);
        }
        #endregion

        #region ManualMoveRequest
        public ManualMoveItemRequestResult Request(ManualMoveItemRequestCommand command)
        {
            return manualMoveSystem.ManualMove(command);
        }
        #endregion

        #region ManualMoveAllItemsRequest
        public ManualMoveAllItemsRequestResult Request(ManualMoveAllItemsRequestCommand command)
        {
            return manualMoveSystem.ManualMove(command);
        }
        #endregion

        #region RemoveItemRequest
        public RemoveItemRequestResult Request(RemoveItemRequestCommand command)
        {
            if(inventoriesHolder.TryGetInventoryByID(command.Inventory, out var inventory))
            {
                inventory.RemoveItem(command.Slot);

                return new RemoveItemRequestResult
                {
                    IsRemoveSuccess = true
                };
            }
            
            return new RemoveItemRequestResult
            {
                IsRemoveSuccess = false
            };
        }
        #endregion

        #region AddItemRequest
        public AddItemRequestResult Request(AddItemRequestCommand command)
        {
            if (inventoriesHolder.TryGetInventoryByID(command.InventoryID, out var inventory))
            {
                var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

                if (itemsHolder.TryGetContainerByID(command.ItemID, out var itemContainer))
                {
                    var itemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType;

                    if (!inventoryMappingConfig.IsInventoryCanContainsType(command.InventoryID, itemType))
                        return new AddItemRequestResult { IsAddSuccess = false };

                    if (inventory.TryGetItemByContainerIndex(command.ItemID, out var item))
                    {
                        if (itemContainer.TryGetComponent<StackableItemTagComponent>(out var stackableComponent))
                        {
                            item.Count += command.Count;
                            return new AddItemRequestResult { IsAddSuccess = true };
                        }
                    }

                    return TryAddNewItemToSlot(command, inventory, itemContainer, command.Count);
                }
            }

            return new AddItemRequestResult { IsAddSuccess = false };
        }

        private AddItemRequestResult TryAddNewItemToSlot(AddItemRequestCommand command, IInventoryComponent inventory, EntityContainer itemContainer, int count)
        {
            if (!inventory.TryGetFreeSlot(command.ItemID, out var slot))
                return new AddItemRequestResult { IsAddSuccess = false };

            var newItem = new InventoryItem
            {
                UniqueID = Guid.NewGuid(),
                ContainerIndex = itemContainer.ContainerIndex,
                Grade = itemContainer.GetComponent<GradeComponent>().Grade,
                ItemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType,
                Count = count,
            };

            inventory.AddItem(slot, newItem);
            Owner.World.Command(new OnAddItemToInventoryCommand() { InventoryID = inventory.InventoryID, Slot = slot, Item = new InventoryItem(newItem) });

            return new AddItemRequestResult { IsAddSuccess = true };
        }

        #endregion
    }

    public struct ManualMoveItemRequestResult
    {
        public bool IsAddSuccess;
    }

    public struct ManualMoveAllItemsRequestResult
    {
        public bool IsAddSuccess;
    }

    public struct AutoMoveItemRequestResult
    {
        public bool IsAddSuccess;
        public int NewSlot;
        public int NewInventoryID;
    }

    public struct RemoveItemRequestResult
    {
        public bool IsRemoveSuccess;
    }

    public struct AddItemRequestResult
    {
        public bool IsAddSuccess;
    }
}

namespace Commands
{
    [Documentation(Doc.Inventory, DocFeature.InventoryFeature, "by this command we clear selected inventory")]
    public struct ClearInventoryCommand : IGlobalCommand
    {
        public int InventoryID;
    }
}