using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, Doc.DragUI,  "StartProcessTouch")]
    public class ClickOrDrag : DragUIBaseState
    {
        private float clickTime = 0.2f * Time.timeScale;
        private float dragLenght = 1;

        public ClickOrDrag(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.ClickOrDrag;

        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            if (!state.IsPressed)
            {
                if (state.PressedTime <= clickTime)
                {
                    stateMachine.ChangeState(DragUIStateIdentifierMap.ProcessClick);
                    return;
                }
            }

            state.PressedTime += Time.deltaTime;

            if (state.PressedTime >= clickTime && state.DragLenght() > dragLenght)
            {
                stateMachine.ChangeState(DragUIStateIdentifierMap.StartDrag);
            }
        }
    }
}
