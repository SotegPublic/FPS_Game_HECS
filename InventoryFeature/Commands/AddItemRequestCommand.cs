using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.Item, "we use this command for add item request to global inventories manager")]
    public struct AddItemRequestCommand : ICommand
    {
        public int ItemID;
        public int Count;
        public int InventoryID;
    }
}