using Commands;
using Components;
using HECSFramework.Core;

namespace Systems
{
    internal class ProcessClick : DragUIBaseState
    {
        private readonly int windowId;

        public ProcessClick(StateMachine stateMachine, int nextDefaultState, Entity owner, int windowId) : base(stateMachine, nextDefaultState, owner)
        {
            this.windowId = windowId;
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.ProcessClick;

        public override void Enter(Entity entity)
        {
            var fromTile = state.CurrentTile;
            var command = new AutoMoveItemRequestCommand
            {
                FromSlot = fromTile.Slot,
                FromInventory = fromTile.InventoryID,
                UIWindowID = windowId,
                InfoOnly = true
            };

            var autoAddRequestResult = entity.World.Request<AutoMoveItemRequestResult, AutoMoveItemRequestCommand>(command);
            if (autoAddRequestResult.IsAddSuccess)
            {
                if (CanAutoMoveItem(command, autoAddRequestResult))
                {
                    command.InfoOnly = false;
                    autoAddRequestResult = entity.World.Request<AutoMoveItemRequestResult, AutoMoveItemRequestCommand>(command);

                    var tilesHolder = entity.GetComponent<InventoriesTilesHolder>();
                    var toTile = tilesHolder.GetTile(autoAddRequestResult.NewInventoryID, autoAddRequestResult.NewSlot);

                    if (toTile)
                    {
                        toTile.ReDrawTile();   
                        fromTile.ReDrawTile();                 
                    }
                }
            }

            EndState();
        }

        protected virtual bool CanAutoMoveItem(AutoMoveItemRequestCommand command, AutoMoveItemRequestResult autoAddRequestResult)
        {
            return true;
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
        }
    }
}
