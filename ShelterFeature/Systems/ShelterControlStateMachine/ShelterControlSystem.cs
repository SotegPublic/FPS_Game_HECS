using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, "ShelterControlSystem")]
    public sealed class ShelterControlSystem : BaseSystem, IReactCommand<InputStartedCommand>,
        IReactCommand<InputPerformedCommand>, IReactCommand<InputEndedCommand>
    {
        [Required] private ShelterControlSystemStateComponent stateComponent;

        private StateMachine stateMachine;

        public override void InitSystem()
        {
            stateMachine = new StateMachine(Owner);

            stateMachine.AddTransition(ShelterControlBaseState.ClickOrMoveState, new IsMove(ShelterControlBaseState.StartMoveState, stateComponent));

            stateMachine.AddState(new AwaitTouchState(stateMachine, ShelterControlBaseState.ClickOrMoveState, Owner));
            stateMachine.AddState(new ClickOrMoveState(stateMachine, ShelterControlBaseState.ProcessClickState, Owner));
            stateMachine.AddState(new ProcessClickState(stateMachine, ShelterControlBaseState.AwaitTouchState, Owner));
            stateMachine.AddState(new StartMoveState(stateMachine, ShelterControlBaseState.MoveInProgressState, Owner));
            stateMachine.AddState(new MoveInProgressState(stateMachine, ShelterControlBaseState.EndMoveState, Owner));
            stateMachine.AddState(new EndMoveState(stateMachine, ShelterControlBaseState.AwaitTouchState, Owner));

            stateMachine.ChangeState(ShelterControlBaseState.AwaitTouchState);
        }

        public void CommandReact(InputStartedCommand command)
        {
            if (!Owner.ContainsMask<ShelterSceneActiveTagComponent>())
                return;
            if (Owner.ContainsMask<BlockShelterSystemTagComponent>())
                return;
            if (!Owner.ContainsMask<MainShelterUIActiveTagComponent>())
                return;

            if (command.Index != InputIdentifierMap.Touch)
                return;

            stateComponent.IsPressed = true;
            stateComponent.From = command.Context.ReadValue<Vector2>();
        }

        public void CommandReact(InputPerformedCommand command)
        {
            if (!Owner.ContainsMask<ShelterSceneActiveTagComponent>())
                return;
            if (Owner.ContainsMask<BlockShelterSystemTagComponent>())
                return;
            if (!Owner.ContainsMask<MainShelterUIActiveTagComponent>())
                return;

            if (command.Index != InputIdentifierMap.Touch)
                return;

            stateComponent.CurrentCoord = command.Context.ReadValue<Vector2>();
        }

        public void CommandReact(InputEndedCommand command)
        {
            if (command.Index != InputIdentifierMap.Touch)
                return;

            stateComponent.IsPressed = false;
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shelter, "StartMoveShelterCameraCommand")]
    public struct StartMoveShelterCameraCommand : IGlobalCommand
    {
        public Vector2 StartPosition;
    }

    [Serializable][Documentation(Doc.Shelter, "MoveShelterCameraCommand")]
    public struct MoveShelterCameraCommand : IGlobalCommand
    {
        public Vector2 CurrentPointerPosition;
    }

    [Serializable][Documentation(Doc.Shelter, "StopMoveShelterCameraCommand")]
    public struct StopMoveShelterCameraCommand : IGlobalCommand
    {
    }
}