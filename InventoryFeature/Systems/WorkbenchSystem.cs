using System;
using Commands;
using Components;
using HECSFramework.Core;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Inventory, "WorkbenchSystem")]
    public sealed class WorkbenchSystem : 
        BaseSystem, 
        IReactGlobalCommand<MergeWorkbenchCommonInventoryCommand>,
        IReactGlobalCommand<ClearWorkbenchInventoriesCommand>,
        IReactGlobalCommand<OnAddItemToInventoryCommand>,
        IReactGlobalCommand<OnRemoveItemFromInventoryCommand>,
        IReactGlobalCommand<OnSwapItemsCommand>
    {
        [Required] private readonly InventoriesHolderComponent playerInventories;

        public override void InitSystem()
        {
            
        }

        public void CommandGlobalReact(MergeWorkbenchCommonInventoryCommand command)
        {
            ClearWorkbenchInventories();

            var toInventory = playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.WorkbenchCommonInventory);

            CopyInventory(toInventory, InventoryTypeIdentifierMap.PlayerArtefactInventory);
            CopyInventory(toInventory, InventoryTypeIdentifierMap.PlayerStorageInventory);
            CopyInventory(toInventory, InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft);
            CopyInventory(toInventory, InventoryTypeIdentifierMap.PlayerWeaponInventoryRight);

            CopyInventory(toInventory, InventoryTypeIdentifierMap.ShelterStorageInventory);
        }

        public void CommandGlobalReact(ClearWorkbenchInventoriesCommand command)
        {
            ClearWorkbenchInventories();
        }

        private void ClearWorkbenchInventories()
        {
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchBasisInventory);
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchAdditionInventory);
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchCommonInventory);
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchResultInventory);
        }

        private void ClearInventory(int inventoryId)
        {
            var inventory = playerInventories.GetInventoryByID(inventoryId);
            inventory.RemoveAll();
        }

        private void CopyInventory(IInventoryComponent toInventory, int fromInventoryId)
        {
            var fromInventory = playerInventories.GetInventoryByID(fromInventoryId);
            using var items = fromInventory.GetItems();

            for (int i = 0; i < items.Count; i++) 
            {
                var value = items.Items[i].Value;

                if (value.IsAvailable)
                    continue;
                if (!toInventory.TryGetFreeSlot(value.ContainerIndex, out var freeSlot))
                    continue;                

                toInventory.AddItem(freeSlot, value);
            }
        }

        public void CommandGlobalReact(OnAddItemToInventoryCommand command)
        {
            switch(command.InventoryID)
            {
                case InventoryTypeIdentifierMap.WorkbenchBasisInventory:
                    SetAdditionFilter();
                    break;

                case InventoryTypeIdentifierMap.WorkbenchAdditionInventory:
                    CreateResult();
                    break;
            }

            Owner.World.Command(new AfterCommand<OnAddItemToInventoryCommand> { Value = command });
        }

        public void CommandGlobalReact(OnRemoveItemFromInventoryCommand command)
        {
            switch(command.InventoryID)
            {
                case InventoryTypeIdentifierMap.WorkbenchBasisInventory:
                    Owner.World.Request<ManualMoveAllItemsRequestResult, ManualMoveAllItemsRequestCommand>
                    (
                        new ManualMoveAllItemsRequestCommand
                        {
                            FromInventory = InventoryTypeIdentifierMap.WorkbenchAdditionInventory,
                            ToInventory = InventoryTypeIdentifierMap.WorkbenchCommonInventory
                        }
                    );

                    SetAdditionFilter();
                    break;

                case InventoryTypeIdentifierMap.WorkbenchAdditionInventory:
                    var itemInfo = Owner.World.Request<ItemInfo, ItemInfoRequest>
                    (
                        new ItemInfoRequest
                        {
                            InventoryIndex = InventoryTypeIdentifierMap.WorkbenchAdditionInventory,
                            SlotIndex = 0
                        }
                    );
                    if (itemInfo.IsAvailable)
                    {
                        ClearInventory(InventoryTypeIdentifierMap.WorkbenchResultInventory);
                    }
                    break;

                case InventoryTypeIdentifierMap.WorkbenchResultInventory:
                    AddItemToInventary(InventoryTypeIdentifierMap.ShelterStorageInventory, command.Item);

                    var targetInventories = new []
                    {
                        playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerArtefactInventory),
                        playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerStorageInventory),
                        playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft),
                        playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerWeaponInventoryRight),
                        playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.ShelterStorageInventory),
                    };
                    RemoveItemsFromInventaries(InventoryTypeIdentifierMap.WorkbenchBasisInventory, targetInventories);
                    RemoveItemsFromInventaries(InventoryTypeIdentifierMap.WorkbenchAdditionInventory, targetInventories);
                    RemoveItemsFromInventaries(InventoryTypeIdentifierMap.WorkbenchResultInventory, targetInventories);
                    
                    ClearWorkbench();
                    break;
            }

            Owner.World.Command(new AfterCommand<OnRemoveItemFromInventoryCommand> { Value = command });
        }

        public void CommandGlobalReact(OnSwapItemsCommand command)
        {
            switch(command.ToInventoryID)
            {
                case InventoryTypeIdentifierMap.WorkbenchBasisInventory:

                    if (command.FromInventoryID == InventoryTypeIdentifierMap.WorkbenchResultInventory)
                    {
                        // можно добавить сложный case с переходом от результата сразу к его прокачке
                        // сейчас это запрещено через WorkbenchEndDrag
                    }
                    else
                    {
                        CommandGlobalReact
                        (
                            new OnRemoveItemFromInventoryCommand 
                            { 
                                InventoryID = command.ToInventoryID, 
                                Slot = command.ToSlot,
                                Item = command.ToItem
                            }
                        );
                    }
                    break;
            }
        }

        private void RemoveItemsFromInventaries(int inventoryId, IInventoryComponent[] targetInventories)
        {
            var sourceInventory = playerInventories.GetInventoryByID(inventoryId);
            using var items = sourceInventory.GetItems();            

            for (int i = 0; i < items.Count; i++) 
            {
                var value = items.Items[i].Value;

                if (value.IsAvailable)
                    continue;

                foreach(var targetInventory in targetInventories)
                {
                    if (targetInventory.TryGetItem(value.UniqueID, out int slot))
                    {
                        targetInventory.RemoveItem(slot);
                        break;
                    }
                }
            }
        }

        private void AddItemToInventary(int inventoryId, InventoryItem item)
        {
            if (item.IsAvailable)
                return;

            var inventory = playerInventories.GetInventoryByID(inventoryId);
            if (inventory.TryGetFreeSlot(item.ContainerIndex, out var slot))
            {
                inventory.AddItem(slot, item);
            }
        }

        private void ClearWorkbench()
        {
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchBasisInventory);
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchAdditionInventory);
            ClearInventory(InventoryTypeIdentifierMap.WorkbenchResultInventory);
        }

        private void CreateResult()
        {
            var upgradeData = GetNextUpgradeData(Owner.World);
            if (!upgradeData.Success)
                return;
            if (!upgradeData.EntityContainer)
                return;

            var inventary = playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.WorkbenchResultInventory);
            inventary.RemoveAll();

            Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            {
                ItemID = upgradeData.EntityContainer.ContainerIndex,
                Count = 1,
                InventoryID = InventoryTypeIdentifierMap.WorkbenchResultInventory
            });
        }

        private void SetAdditionFilter()
        {
            var upgradeData = GetCurrentUpgradeData(Owner.World);
            var workbenchAdditionInventory = 
                playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.WorkbenchAdditionInventory) as IInventoryWithFilterComponent;

            workbenchAdditionInventory.SetFilter(upgradeData.Success, upgradeData.EntityContainer ? upgradeData.EntityContainer.ContainerIndex : default);
        }

        private static UpgradeData GetCurrentUpgradeData(World world)
        {
            return GetUpgradeData(world, 0);
        }

        private static UpgradeData GetNextUpgradeData(World world)
        {
            return GetUpgradeData(world, 1);
        }

        private static UpgradeData GetUpgradeData(World world, int upgradeLevel)
        {
            var itemInfo = world.Request<ItemInfo, ItemInfoRequest>
            (
                new ItemInfoRequest
                {
                    InventoryIndex = InventoryTypeIdentifierMap.WorkbenchBasisInventory,
                    SlotIndex = 0
                }
            );
            if (itemInfo.IsAvailable)
            {
                return new UpgradeData { Success = true };
            }

            var upgradeRootIndexComponent = itemInfo.EntityContainer.GetComponent<UpgradeRootIndexComponent>();
            if (upgradeRootIndexComponent == null)
            {
                return default;
            }

            return world.Request<UpgradeData, UpgradeInfoCommand>
            (
                new UpgradeInfoCommand
                {
                    RootContainerIndex = upgradeRootIndexComponent.RootContainerIndex,
                    UpgradeLevel = upgradeRootIndexComponent.UpgradeLevel + upgradeLevel
                }
            );
        }
    }
}

namespace Commands
{
    [Documentation(Doc.Inventory, "MergeWorkbenchCommonInventoryCommand")]
    public struct MergeWorkbenchCommonInventoryCommand : IGlobalCommand
    {
        
    }

    [Documentation(Doc.Inventory, "ClearWorkbenchInventoriesCommand")]
    public struct ClearWorkbenchInventoriesCommand : IGlobalCommand
    {
        
    }
}