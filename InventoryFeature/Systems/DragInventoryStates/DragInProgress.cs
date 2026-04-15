using Components;
using HECSFramework.Core;
using Helpers;
using UnityEngine;

namespace Systems
{
    public class DragInProgress : DragUIBaseState
    {
        private RectTransform canvas;

        public DragInProgress(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
            canvas = owner.World.GetSingleComponent<MainCanvasTagComponent>().Actor.transform.AsRectTransform();
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.DragInProgress;

        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas,
                state.CurrentCoord,
                null,
                out var localPoint
            );

            state.DragView.RectTransform.localPosition = localPoint;

            if (!state.IsPressed)
            {
                state.DragView.gameObject.SetActive(false);
                EndState();
            }
        }
    }
}
