using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Inventory, Doc.Item, "we use this command for move all items request to global inventories manager")]
    public struct ManualMoveAllItemsRequestCommand : ICommand
    {
        public int ToInventory;
        public int FromInventory;
    }
}