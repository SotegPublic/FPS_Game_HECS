using System;
using Commands;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, Doc.DragUI, "WorkbenchEndDrag")]
    public class WorkbenchEndDrag : EndDrag
    {
        private readonly Func<ManualMoveItemRequestCommand, bool> canMoveItemFunc;

        public WorkbenchEndDrag
        (
            StateMachine stateMachine, 
            int nextDefaultState, 
            Entity owner, 
            Func<ManualMoveItemRequestCommand, bool> canMoveItemFunc
        ) : 
        base(stateMachine, nextDefaultState, owner)
        {
            this.canMoveItemFunc = canMoveItemFunc;
        }

        protected override bool CanManualMoveItem(ManualMoveItemRequestCommand command)
        {
            return canMoveItemFunc(command);
        }
    }
}
