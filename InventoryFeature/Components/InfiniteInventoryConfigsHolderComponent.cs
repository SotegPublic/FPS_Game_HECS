using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, Doc.Config, "config for inventory system with infinite inventories")]
    public sealed class InfiniteInventoryConfigsHolderComponent : BaseComponent
    {
        [SerializeField] private InfiniteInventoryConfig[] inventoryConfigs;

        public InfiniteInventoryConfig[] InventoryConfigs => inventoryConfigs;
    }

    [Serializable]
    public class InfiniteInventoryConfig
    {
        [SerializeField] private int slotCount;
        [SerializeField] private int multiplicity;
        [SerializeField][IdentifierDropDown(nameof(InventoryTypeIdentifier))] private int inventoryTypeID;

        public int SlotCount => slotCount;
        public int Multiplicity => multiplicity;
        public int InventoryTypeID => inventoryTypeID;
    }
}