using Commands;
using Components;
using Components.MonoBehaviourComponents;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    [Documentation(Doc.Visual, Doc.Inventory, Doc.UI, "LootAndInventoryUIDragSystem")]
    [RequiredAtContainer(typeof(InputListenerTagComponent))]
    public class LootAndInventoryUIDragSystem : BaseSystem, IItemDragSystem, IHaveActor
    {
        [Required]
        public DragInventoryStateComponent StateComponent;

        [Required]
        public UIAccessProviderComponent UIAccessProviderComponent;
        [Required]
        public UITagComponent UITagComponent;

        private InputAction touchPosition;
        protected StateMachine stateMachine;

        public Actor Actor { get; set; }

        public override void InitSystem()
        {
            Owner.World.GetSingleComponent<InputActionsComponent>()
                .TryGetInputAction(InputIdentifierMap.Touch, out touchPosition);
            
            stateMachine = new StateMachine(Owner);
            PrepareStates();

            var prfb = Actor.GetComponent<UIAccessPrefabs>().GetPrefab(UIAccessIdentifierMap.ItemTile);
            var root = Actor.GetComponent<UIAccessMonoComponent>().GetRectTransform(UIAccessIdentifierMap.Root);

            var itemInfoSource = new BaseItemInfoSource(Owner.World);

            StateComponent.DragView = Object.Instantiate(prfb, root).GetComponent<ItemTileMonoComponent>();
            StateComponent.DragView.Init(itemInfoSource);
            StateComponent.DragView.DisableBackground();
            StateComponent.DragView.gameObject.SetActive(false);
        }

        protected virtual void PrepareStates()
        {
            stateMachine.AddState(new StartProcessTouch(stateMachine, DragUIStateIdentifierMap.ClickOrDrag, Owner));
            stateMachine.AddState(new ClickOrDrag(stateMachine, DragUIStateIdentifierMap.ProcessClick, Owner));
            stateMachine.AddState(new ProcessClick(stateMachine, DragUIStateIdentifierMap.StartProcessTouch, Owner, UITagComponent.ViewType));
            stateMachine.AddState(new StartDrag(stateMachine, DragUIStateIdentifierMap.DragInProgress, Owner));
            stateMachine.AddState(new DragInProgress(stateMachine, DragUIStateIdentifierMap.EndDrag, Owner));
            stateMachine.AddState(new EndDrag(stateMachine, DragUIStateIdentifierMap.StartProcessTouch, Owner));

            stateMachine.ChangeState(DragUIStateIdentifierMap.StartProcessTouch);
        }

        public void CommandReact(InputStartedCommand command)
        {
            if (command.Index == InputIdentifierMap.Touch)
            {
                StateComponent.IsPressed = true;
                StateComponent.IsPressedChecked = false;
                StateComponent.From = touchPosition.ReadValue<Vector2>();
                StateComponent.PressedTime = 0;
            }
        }

        public void CommandReact(InputEndedCommand command)
        {
            if (command.Index == InputIdentifierMap.Touch)
            {
                StateComponent.IsPressed = false;
            }
        }

        public void CommandReact(InputPerformedCommand command)
        {
            if (command.Index == InputIdentifierMap.Touch)
            {
                StateComponent.CurrentCoord = touchPosition.ReadValue<Vector2>();
                StateComponent.PressedTime += Time.deltaTime;
            }
        }
    }

    public interface IItemDragSystem : ISystem,
        IReactCommand<InputStartedCommand>,
        IReactCommand<InputEndedCommand>,
        IReactCommand<InputPerformedCommand>
    { }
}
