using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, Doc.Config, "config for inventory system")]
    public sealed class InventoryConfigsHolderComponent : BaseComponent
    {
        [SerializeField] private InventoryConfig[] inventoryConfigs;

        public InventoryConfig[] InventoryConfigs => inventoryConfigs;
    }

    [Serializable]
    public class InventoryConfig
    {
        [SerializeField] private int slotsCount;
        [SerializeField][IdentifierDropDown(nameof(InventoryTypeIdentifier))] private int inventoryTypeID;

        public int SlotsCount => slotsCount;
        public int InventoryTypeID => inventoryTypeID;
    }
}