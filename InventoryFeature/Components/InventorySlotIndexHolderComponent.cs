using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, Doc.Abilities, "here we hold inventory slot index in which the weapon is located")]
    public sealed class InventorySlotIndexHolderComponent : BaseComponent
    {
       public int SlotIndex;
        public int InventoryID;


        public void SetValues(int slot, int inventoryID)
        {
            SlotIndex = slot;
            InventoryID = inventoryID;
        }
    }
}