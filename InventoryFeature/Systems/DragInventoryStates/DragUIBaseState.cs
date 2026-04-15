using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, "StartProcessTouch")]
    public abstract class DragUIBaseState : BaseFSMState
    {
        protected DragInventoryStateComponent state;

        protected DragUIBaseState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState)
        {
            state = owner.GetComponent<DragInventoryStateComponent>();
        }
    }
}
