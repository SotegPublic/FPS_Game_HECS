using Components;
using Components.MonoBehaviourComponents;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, Doc.DragUI, "StartProcessTouch")]
    public class StartProcessTouch : DragUIBaseState
    {
        public StartProcessTouch(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
            state = owner.GetComponent<DragInventoryStateComponent>();
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.StartProcessTouch;

      
        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            if (state.IsPressed && !state.IsPressedChecked)
            {
                if (entity.World.TryGetSingleComponent(out InputOverUIComponent inputOverUIComponent))
                {
                    for (int i = 0; i < inputOverUIComponent.RaycastResults.Count; i++)
                    {
                        if (inputOverUIComponent.RaycastResults[i].gameObject.TryGetComponent(out ItemTileMonoComponent inventoryTileMonoComponent))
                        {
                            state.CurrentTile = inventoryTileMonoComponent;

                            EndState();
                        }
                    } 
                }

                state.IsPressedChecked = true;
            }
        }
    }
}