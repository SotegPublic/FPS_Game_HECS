using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ResetElevatorState")]
    public class ResetElevatorState : ShelterElevatorBaseState
    {
        private ElevatorSystemStateComponent elevatorSystemStateComponent;

        public ResetElevatorState(StateMachine stateMachine, int nextDefaultState,
            ElevatorSystemStateComponent stateComponent) : base(stateMachine, nextDefaultState)
        {
            elevatorSystemStateComponent = stateComponent;
        }

        public override int StateID => ShelterElevatorBaseState.ResetElevatorState;

        public override void Enter(Entity entity)
        {
            entity.GetTransform().position = elevatorSystemStateComponent.ElevatorBasePosition;
            elevatorSystemStateComponent.ElevatorProgress = 0f;
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