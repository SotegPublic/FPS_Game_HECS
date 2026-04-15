using System;
using HECSFramework.Core;

namespace Components
{
    public interface IInventoryWithFilterComponent : IInventoryComponent
    {
        public int FilterContainerIndex { get; }
        public bool IsFilterSet { get; }
        public void SetFilter(bool isValid, int containerIndex = default);
        public bool IsMatches(int containerIndex);
    }

    [Serializable][Documentation(DocFeature.InventoryFeature, "BaseInventoryWithFilterComponent")]
    public abstract class BaseInventoryWithFilterComponent : InventoryComponent, IInventoryWithFilterComponent
    {
        private const int INVALID_CONTAINER_INDEX = -1;

        public int FilterContainerIndex { get; private set; }
        public bool IsFilterSet => FilterContainerIndex != default;

        public override void AddItem(int inventorySlot, InventoryItem item)
        {
            if (IsMatches(item))
            {
                base.AddItem(inventorySlot, item);
            }
        }

        public override void RemoveAll()
        {
            FilterContainerIndex = default;
            base.RemoveAll();
        }

        private bool IsMatches(InventoryItem item)
        {
            return item.IsAvailable || IsMatches(item.ContainerIndex);
        }

        public bool IsMatches(int containerIndex)
        {
            return FilterContainerIndex == default || FilterContainerIndex == containerIndex;
        }

        public void SetFilter(bool isValid, int containerIndex = default)
        {
            if (isValid)
            {
                FilterContainerIndex = containerIndex;
            }
            else
            {
                FilterContainerIndex = INVALID_CONTAINER_INDEX;
            }
        }
    }
}