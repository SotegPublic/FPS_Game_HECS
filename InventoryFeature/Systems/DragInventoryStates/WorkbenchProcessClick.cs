using System;
using Commands;
using HECSFramework.Core;

namespace Systems
{
    internal sealed class WorkbenchProcessClick : ProcessClick
    {
        private readonly Func<ManualMoveItemRequestCommand, bool> canMoveItemFunc;

        public WorkbenchProcessClick
        (
            StateMachine stateMachine, 
            int nextDefaultState, 
            Entity owner, 
            int windowId, 
            Func<ManualMoveItemRequestCommand, bool> canMoveItemFunc
        ) : 
        base(stateMachine, nextDefaultState, owner, windowId)
        {
            this.canMoveItemFunc = canMoveItemFunc;
        }

        protected override bool CanAutoMoveItem(AutoMoveItemRequestCommand command, AutoMoveItemRequestResult autoAddRequestResult)
        {
            var moveCommand = new ManualMoveItemRequestCommand
            {
                FromInventory = command.FromInventory,
                ToInventory = autoAddRequestResult.NewInventoryID,
                FromSlot = command.FromSlot,
                ToSlot = autoAddRequestResult.NewSlot
            };

            return canMoveItemFunc(moveCommand);
        }
    }
}
