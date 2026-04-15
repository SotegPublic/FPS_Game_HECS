using System;
using System.Collections.Generic;
using HECSFramework.Core;
using Helpers;

public static partial class DocFeature
{
    public const string InventoryFeature = "InventoryFeature";
}

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "base inventory component")]
    public abstract partial class InventoryComponent : BaseComponent, IInventoryComponent
    {
        [Field(0)]
        protected Dictionary<int, InventoryItem> items = new Dictionary<int, InventoryItem>();

        protected bool isInfinite;
        protected int multiplicity;

        public abstract int InventoryID { get; }
        public int Count => items.Count;

        public virtual void CreateInventory(int slotsCount, bool isInfinite = false, int multiplicity = 0)
        {
            if (items.Count != 0)
                return;
            
            this.isInfinite = isInfinite;
            this.multiplicity = multiplicity;

            for (int i = 0; i < slotsCount; i++)
            {
                GetOrAddItem(i);
            }

            CheckSlotCount();
        }

        protected virtual InventoryItem GetOrAddItem(int slotIndex)
        {
            if (items.TryGetValue(slotIndex, out var slotItem))
            {
                if (slotItem != null)
                {
                    return slotItem;
                }
            }

            items[slotIndex] = new InventoryItem
            {
                UniqueID = Guid.Empty,
                Count = 0,
                ContainerIndex = 0,
                Grade = 0,
                ItemType = 0
            };

            return items[slotIndex];
        }

        private void CheckSlotCount()
        {
            if (!isInfinite)
                return;

            var oldCount = Count;
            var newCount = Count;

            if (multiplicity > 1)
            {
                var group = newCount / multiplicity;
                if (newCount > group * multiplicity)
                {
                    group++;
                }
                newCount = group * multiplicity;
            }

            for (int i = oldCount; i < newCount; i++)
            {
                GetOrAddItem(i);
            }
        }

        public virtual void AddItem(int inventorySlot, InventoryItem item)
        {
            GetOrAddItem(inventorySlot).Copy(item);
            CheckSlotCount();
        }

        public virtual void RemoveItem(int inventorySlot)
        {
            items[inventorySlot].CleanData();
        }

        public virtual void SwapItems(int firstSlot, int secondSlot)
        {
            var firstItem = GetOrAddItem(firstSlot);
            var secondItem = GetOrAddItem(secondSlot);

            items[firstSlot] = secondItem;
            items[secondSlot] = firstItem;
        }

        public virtual void MoveItem(int toSlot, int fromSlot)
        {
            GetOrAddItem(toSlot).Copy(items[fromSlot]);
            GetOrAddItem(fromSlot).CleanData();
        }

        public virtual bool TryGetFreeSlot(int itemID, out int freeSlot)
        {
            foreach (var element in items)
            {
                if (element.Value == null || element.Value.IsAvailable)
                {
                    freeSlot = element.Key;
                    return true;
                }
            }

            if (isInfinite)
            {
                freeSlot = Count;
                GetOrAddItem(Count);
                return true;
            }

            freeSlot = default;
            return false;
        }

        public InventoryItem GetItem(int slotIndex)
        {
            // if (isInfinite)
            // {
            //     return GetOrAddItem(slotIndex);
            // }

            return items[slotIndex];
        }

        public bool TryGetSlotWithSameItem(int itemID, out int slotIndex)
        {
            foreach (var item in items)
            {
                if (item.Value == null || item.Value.IsAvailable)
                    continue;

                if (item.Value.ContainerIndex == itemID)
                {
                    slotIndex = item.Key;
                    return true;
                }
            }

            slotIndex = default;
            return false;
        }

        public bool TryGetItem(int slotIndex, out InventoryItem inventoryItem)
        {
            if (items.TryGetValue(slotIndex, out inventoryItem) && inventoryItem != null && !inventoryItem.IsAvailable)
            {
                return true;
            }

            inventoryItem = null;
            return false;   
        }

        public bool TryGetItem(Guid guid, out InventoryItem inventoryItem)
        {   
            foreach(var item in items)
            {
                if (item.Value.UniqueID == guid)
                {
                    inventoryItem = item.Value;
                    return true;
                }
            }

            inventoryItem = null;
            return false;   
        }

        public bool TryGetItem(Guid guid, out int slotIndex)
        {   
            foreach(var item in items)
            {
                if (item.Value.UniqueID == guid)
                {
                    slotIndex = item.Key;
                    return true;
                }
            }

            slotIndex = default;
            return false;   
        }

        public HECSPooledArray<KeyValuePair<int, InventoryItem>> GetItems()
        {
            var array = HECSPooledArray<KeyValuePair<int, InventoryItem>>.GetArray(items.Count);

            foreach (var item in items)
                array.Add(item);

            return array;
        }

        public bool TryGetItemByContainerIndex(int containerIndex, out InventoryItem inventoryItem)
        {
            foreach (var item in items)
            {
                if (item.Value == null || item.Value.IsAvailable)
                    continue;

                if (item.Value.ContainerIndex == containerIndex)
                {
                    inventoryItem = item.Value;
                    return true;
                }
            }

            inventoryItem = null;
            return false;
        }

        public virtual void RemoveAll()
        {
            var slotsCount = Count;
            for (int i = 0; i < slotsCount; i++)
            {
                RemoveItem(i);
            }
        }
    }
    public interface IInventoryComponent : IComponent
    {
        public int InventoryID { get; }

        public void CreateInventory(int slotsCount, bool isInfinite = false, int multiplicity = 0);

        public void AddItem(int inventorySlot, InventoryItem item);
        public void RemoveItem(int inventorySlot);
        public void MoveItem(int toSlot, int fromSlot);


        public void RemoveAll();
        public void SwapItems(int firstSlot, int secondSlot);

        public bool TryGetFreeSlot(int itemID, out int freeSlot);
        public bool TryGetSlotWithSameItem(int itemID, out int slotIndex);

        public InventoryItem GetItem(int slotIndex);
        public bool TryGetItem(int slotIndex, out InventoryItem inventoryItem);
        public bool TryGetItem(Guid uniqueID, out InventoryItem inventoryItem);
        public bool TryGetItem(Guid uniqueID, out int slotIndex);
        public bool TryGetItemByContainerIndex(int containerIndex, out InventoryItem inventoryItem);

        public HECSPooledArray<KeyValuePair<int, InventoryItem>> GetItems();
    }

    public class InventoryItem : IEquatable<InventoryItem>
    {
        public Guid UniqueID;
        public int ContainerIndex;
        public int ItemType;
        public int Grade;
        public int Count;

        public bool IsAvailable => ContainerIndex == 0;

        public InventoryItem() 
        { 

        }

        public InventoryItem(InventoryItem item) 
        { 
            Copy(item);
        }

        public void CleanData()
        {
            UniqueID = Guid.Empty;
            Count = 0;
            ContainerIndex = 0;
            Grade = 0;
            ItemType = 0;
        }

        public void Copy(InventoryItem item)
        {
            UniqueID = item.UniqueID;
            Count = item.Count;
            ContainerIndex = item.ContainerIndex;
            Grade = item.Grade;
            ItemType = item.ItemType;
        }

        public override bool Equals(object obj)
        {
            return obj is InventoryItem item &&
                   UniqueID.Equals(item.UniqueID) && ContainerIndex == item.ContainerIndex;
        }

        public bool Equals(InventoryItem other)
        {
            return UniqueID.Equals(other.UniqueID) && ContainerIndex == other.ContainerIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UniqueID);
        }
    }
}