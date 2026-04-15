using System;
using Commands;
using Components;
using Components.MonoBehaviourComponents;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, Doc.DragUI, "EndDrag")]
    public class EndDrag : DragUIBaseState
    {
        public EndDrag(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.EndDrag;

        public override void Enter(Entity entity)
        {
            var overUI = entity.World.GetSingleComponent<InputOverUIComponent>();

            if (overUI.InputOverUI(state.CurrentCoord))
            {
                foreach (var uiElement in overUI.RaycastResults)
                {
                    if (uiElement.gameObject.TryGetComponent<ItemTileMonoComponent>(out var toTile) && toTile != state.CurrentTile)
                    {
                        var fromTile = state.CurrentTile;
                        var command = new ManualMoveItemRequestCommand
                        {
                            FromInventory = fromTile.InventoryID,
                            ToInventory = toTile.InventoryID,
                            FromSlot = fromTile.Slot,
                            ToSlot = toTile.Slot
                        };

                        if (!CanManualMoveItem(command))
                            continue;

                        var moveItemRequest = entity.World.Request<ManualMoveItemRequestResult, ManualMoveItemRequestCommand>(command);
                        if(moveItemRequest.IsAddSuccess)
                        {
                            toTile.ReDrawTile();
                            fromTile.ReDrawTile();

                            EndState();
                            return;
                        }
                    }
                }

                ReturnItemTile();
            }
            else
            {
                ReturnItemTile();
            }

            EndState();
        }

        protected virtual bool CanManualMoveItem(ManualMoveItemRequestCommand command)
        {
            return true;
        }

        private void ReturnItemTile()
        {
            state.DragView.gameObject.SetActive(false);
            state.CurrentTile.ReDrawTile();    
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
        }
    }
}
