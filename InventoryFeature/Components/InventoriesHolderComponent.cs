using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Inventory, DocFeature.InventoryFeature, Doc.Config, "here we hold all inventoryComponents from entity")]
    public sealed class InventoriesHolderComponent : BaseComponent
    {
        private HECSList<IInventoryComponent> inventoryComponents = new HECSList<IInventoryComponent>(8);

        public void AddInventory(IInventoryComponent inventoryComponent)
        {
            inventoryComponents.Add(inventoryComponent);
        }

        public bool TryGetInventoryByID(int inventoryID, out IInventoryComponent inventoryComponent)
        {
            for(int i = 0; i < inventoryComponents.Count; i++)
            {
                if (inventoryComponents[i].InventoryID == inventoryID)
                {
                    inventoryComponent = inventoryComponents[i];
                    return true;
                }
            }

            inventoryComponent = null;
            return false;
        }

        public IInventoryComponent GetInventoryByID(int inventoryID)
        {
            for (int i = 0; i < inventoryComponents.Count; i++)
            {
                if (inventoryComponents[i].InventoryID == inventoryID)
                {
                    return inventoryComponents[i];
                }
            }

            return null;
        }

        public void GetInventoriesIDsNonAlloc(HECSList<int> identifiersList)
        {
            identifiersList.ClearFast();
            for(int i = 0; i < inventoryComponents.Count; i++)
            {
                identifiersList.Add(inventoryComponents[i].InventoryID);
            }
        }
    }
}