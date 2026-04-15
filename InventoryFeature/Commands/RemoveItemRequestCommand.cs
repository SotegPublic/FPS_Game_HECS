using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.Item, "we use this command for remove item request to global inventories manager")]
    public struct RemoveItemRequestCommand : ICommand
    {
        public int Slot;
        public int Inventory;
    }
}