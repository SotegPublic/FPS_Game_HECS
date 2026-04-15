using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ElevatorReturnState")]
    public class ElevatorReturnState : ShelterElevatorBaseState
    {
        private ElevatorConfigComponent elevatorConfigComponent;
        private ElevatorSystemStateComponent elevatorSystemStateComponent;

        private Vector3 startPosition;

        public ElevatorReturnState(StateMachine stateMachine, int nextDefaultState,
            ElevatorSystemStateComponent stateComponent, ElevatorConfigComponent config) : base(stateMachine, nextDefaultState)
        {
            elevatorConfigComponent = config;
            elevatorSystemStateComponent = stateComponent;
        }

        public override int StateID => ShelterElevatorBaseState.ElevatorReturnState;

        public override void Enter(Entity entity)
        {
            startPosition = entity.GetPosition();
        }

        public override void Exit(Entity entity)
        {
            elevatorSystemStateComponent.ElevatorProgress = 0f;
        }

        public override void Update(Entity entity)
        {
            MoveElevator(entity);

            if (elevatorSystemStateComponent.ElevatorProgress >= 1)
            {
                EndState();
            }
        }

        private void MoveElevator(Entity entity)
        {
            var speed = elevatorConfigComponent.ElevatorSpeed * Time.deltaTime;
            entity.GetTransform().position = Vector3.MoveTowards(entity.GetPosition(), elevatorSystemStateComponent.ElevatorBasePosition, speed);

            var coveredDistance = Mathf.Abs(entity.GetPosition().y - startPosition.y); 
            elevatorSystemStateComponent.ElevatorProgress = coveredDistance / Mathf.Abs(startPosition.y - elevatorSystemStateComponent.ElevatorBasePosition.y);
        }
    }
}