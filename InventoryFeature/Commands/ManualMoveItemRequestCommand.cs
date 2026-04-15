using Components;
using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.Item, "we use this command for move item request to global inventories manager")]
    public struct ManualMoveItemRequestCommand : ICommand
    {
        public int ToSlot;
        public int ToInventory;
        public int FromSlot;
        public int FromInventory;
    }
}