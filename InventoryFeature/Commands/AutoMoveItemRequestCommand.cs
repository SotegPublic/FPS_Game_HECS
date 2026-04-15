using Components;
using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.Item, "we use this command for auto add item request to global inventories manager")]
    public struct AutoMoveItemRequestCommand : ICommand
    {
        public int FromSlot;
        public int FromInventory;
        public int UIWindowID;
        public bool InfoOnly;
    }
}