using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Helpers;
using BluePrints.Identifiers;
using System.Collections.Generic;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, Doc.Config, "here we map inventory-items relations")]
    public sealed class InventoryMappingConfigComonent : BaseComponent
    {
        [SerializeField] private InventoryMap[] inventoriesMaps;

        private Dictionary<int, HashSet<int>> maps;

        public override void Init()
        {
            base.Init();

            maps = new Dictionary<int, HashSet<int>>(inventoriesMaps.Length);

            for (int i = 0; i < inventoriesMaps.Length; i++)
            {
                maps.Add(inventoriesMaps[i].InventoryID, new HashSet<int>(inventoriesMaps[i].ItemTypes));
            }
        }

        public bool IsInventoryCanContainsType(int inventoryID, int itemType)
        {
            return maps[inventoryID].Contains(LootItemTypeIdentifierMap.All) || maps[inventoryID].Contains(itemType);
        }
    }

    [Serializable]
    public class InventoryMap
    {
        [IdentifierDropDown(nameof(InventoryTypeIdentifier))] public int InventoryID;
        [IdentifierDropDown(nameof(LootItemTypeIdentifier))] public int[] ItemTypes;
    }
}