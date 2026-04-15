using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, "this system control shelter elevator")]
    public sealed class ShelterElevatorSystem : BaseSystem, IReactGlobalCommand<CleanShelterGlobalCommand>
    {
        [Required] private PassengerHolderComponent passengerHolder;
        [Required] private ElevatorSystemStateComponent stateComponent;
        [Required] private ElevatorConfigComponent configComponent;

        private StateMachine stateMachine;

        public void CommandGlobalReact(CleanShelterGlobalCommand command)
        {
            passengerHolder.Clear();
            stateMachine.ChangeState(ShelterElevatorBaseState.ResetElevatorState);
            stateMachine.Dispose();

            stateMachine = null;
        }

        public override void InitSystem()
        {
            var arrivedSurvivorsHolderComponent = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>().GetComponent<ArrivedSurvivorsHolderComponent>();
            stateComponent.ElevatorBasePosition = Owner.GetPosition();

            stateMachine = new StateMachine(Owner);

            stateMachine.AddState(new AwaitArrivedSurvivorState(stateMachine, ShelterElevatorBaseState.GoToTargetRoomState, arrivedSurvivorsHolderComponent, passengerHolder, configComponent));
            stateMachine.AddState(new GoToTargetRoomState(stateMachine, ShelterElevatorBaseState.MoveSurvivorToRoomState, passengerHolder, stateComponent, configComponent));
            stateMachine.AddState(new MoveSurvivorToRoomState(stateMachine, ShelterElevatorBaseState.ElevatorReturnState, passengerHolder, configComponent));
            stateMachine.AddState(new ElevatorReturnState(stateMachine, ShelterElevatorBaseState.AwaitArrivedSurvivorState, stateComponent, configComponent));
            stateMachine.AddState(new ResetElevatorState(stateMachine, 0, stateComponent));

            stateMachine.ChangeState(ShelterElevatorBaseState.AwaitArrivedSurvivorState);
        }
    }
}