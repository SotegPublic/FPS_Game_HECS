using Components;
using HECSFramework.Core;
using Helpers;
using UnityEngine;

namespace Systems
{
    [Documentation(Doc.UI, Doc.State, Doc.Inventory, Doc.DragUI, "DragUIBaseState")]
    public class StartDrag : DragUIBaseState
    {
        private readonly RectTransform canvas;

        public StartDrag(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
            canvas = owner.World.GetSingleComponent<MainCanvasTagComponent>().Actor.transform.AsRectTransform();
        }

        public override int StateID { get; } = DragUIStateIdentifierMap.StartDrag;

        public override void Enter(Entity entity)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas,
                state.CurrentCoord,
                null,
                out var localPoint
            );

            state.DragView.RectTransform.localPosition = localPoint;

            state.DragView.gameObject.SetActive(true);

            state.DragView.DrawTile(state.CurrentTile.Slot, state.CurrentTile.InventoryID);
            state.CurrentTile.DisableIcon();
            EndState();
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
        }
    }
}
