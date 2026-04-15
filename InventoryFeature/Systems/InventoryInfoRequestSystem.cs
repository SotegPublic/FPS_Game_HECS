using System;
using System.Linq;
using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;

namespace Systems
{
    public delegate bool GetContainerDelegate(int id, out EntityContainer entityContainer);

	[Serializable][Documentation(Doc.Inventory, Doc.Request, "this system process request to get info about item or loot in slot, we return container if slot is not empty ")]
    public sealed class InventoryInfoRequestSystem : 
        BaseSystem, 
        IRequestProvider<ItemInfo, ItemInfoRequest>, 
        IRequestProvider<InventoryItem, ItemInfoRequest>, 
        IRequestProvider<Guid, ItemInfoRequest>
    {
        private EntitiesFilter inventoryHolderEntities;

        public override void InitSystem()
        {
            inventoryHolderEntities = Owner.World.GetFilter<InventoriesHolderComponent>();
        }

        ItemInfo IRequestProvider<ItemInfo, ItemInfoRequest>.Request(ItemInfoRequest command)
        {
            var itemsHolder = Owner.World.GetSingleComponent<EquipItemsHolderComponent>();
            var inventory = GetInventory(command.InventoryIndex);

            return GetItemInfo(inventory, command, itemsHolder.TryGetContainerByID);
        }

        InventoryItem IRequestProvider<InventoryItem, ItemInfoRequest>.Request(ItemInfoRequest command)
        {
            var inventory = GetInventory(command.InventoryIndex);
            if (inventory.TryGetItem(command.SlotIndex, out var item))
            {
                return item;
            }

            return default;
        }

        Guid IRequestProvider<Guid, ItemInfoRequest>.Request(ItemInfoRequest command)
        {
            var inventory = GetInventory(command.InventoryIndex);
            if (inventory.TryGetItem(command.SlotIndex, out var item))
            {
                return item.UniqueID;
            }

            return default;
        }

        private IInventoryComponent GetInventory(int inventoryIndex)
        {
            // inventoryHolderEntities.ForceUpdateFilter();

            IInventoryComponent inventory = null;
            foreach (var inventoryHolderEntity in inventoryHolderEntities)
            {
                if (inventoryHolderEntity.TryGetComponent(out InventoriesHolderComponent inventoriesHolderComponent))
                {
                    inventory = inventoriesHolderComponent.GetInventoryByID(inventoryIndex);
                    if (inventory != null)
                        break;
                }
            }

            return inventory;
        }

        private ItemInfo GetItemInfo(IInventoryComponent inventory, ItemInfoRequest command, GetContainerDelegate getContainerDelegate)
        {
            if (inventory.TryGetItem(command.SlotIndex, out var inventoryItem))
            {
                if (getContainerDelegate(inventoryItem.ContainerIndex, out var container))
                {
                    return new ItemInfo
                    {
                        Count = inventoryItem.Count,
                        EntityContainer = container,
                        Grade = inventoryItem.Grade,
                        IsAvailable = false,
                    };
                }
            }

            return new ItemInfo { IsAvailable = true };
        }
    }
}

namespace Commands
{
    public struct ItemInfo
    {
        public EntityContainer EntityContainer;
        public int Grade;
        public int Count;
        public bool IsAvailable;
        
        public bool IsTransparent;        
        public bool HasUpgrade;
        public bool IsUse;

        public readonly bool TryGetIcon(out UnityEngine.Sprite sprite)
        {
            if (EntityContainer.TryGetComponent(out IconComponent icon))
            {
                sprite = icon.Icon;
                return true;
            }

            sprite = default;
            return false;
        }
    }

    public struct ItemInfoRequest : ICommand
    {
        public int SlotIndex;
        public int InventoryIndex;
    }
}