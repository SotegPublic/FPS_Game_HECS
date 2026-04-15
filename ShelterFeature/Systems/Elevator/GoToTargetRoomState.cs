using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "GoToTargetRoomState")]
    public class GoToTargetRoomState : ShelterElevatorBaseState
    {
        private PassengerHolderComponent passengerHolderComponent;
        private ElevatorConfigComponent elevatorConfigComponent;
        private ElevatorSystemStateComponent elevatorSystemStateComponent;

        private Vector3 targetPosition;

        public GoToTargetRoomState(StateMachine stateMachine, int nextDefaultState,
            PassengerHolderComponent passengerHolder, ElevatorSystemStateComponent stateComponent, ElevatorConfigComponent config) : base(stateMachine, nextDefaultState)
        {
            passengerHolderComponent = passengerHolder;
            elevatorConfigComponent = config;
            elevatorSystemStateComponent = stateComponent;
        }

        public override int StateID => ShelterElevatorBaseState.GoToTargetRoomState;

        public override void Enter(Entity entity)
        {
            var elevatorPosition = entity.GetPosition();
            var targetRoomFloor = passengerHolderComponent.TargetRoom.GetComponent<RoomMonocomponentProviderComponent>().Get.FloorEdge;
            targetPosition = new Vector3(elevatorPosition.x, targetRoomFloor.position.y, elevatorPosition.z);
        }

        public override void Exit(Entity entity)
        {
            elevatorSystemStateComponent.ElevatorProgress = 0f;
            targetPosition = Vector3.zero;
        }

        public override void Update(Entity entity)
        {
            MoveElevator(entity);
            MovePassenger(entity);

            if (elevatorSystemStateComponent.ElevatorProgress >= 1)
            {
                EndState();
            }
        }

        private void MovePassenger(Entity entity)
        {
            var elevatorPosition = entity.GetPosition();
            elevatorPosition.y += elevatorConfigComponent.ElevatorFloorOffset;
            passengerHolderComponent.Passenger.Entity.GetTransform().position = elevatorPosition;
            passengerHolderComponent.Passenger.Entity.GetTransform().rotation = Quaternion.Euler(0,180,0);
        }

        private void MoveElevator(Entity entity)
        {
            var speed = elevatorConfigComponent.ElevatorSpeed * Time.deltaTime;
            entity.GetTransform().position = Vector3.MoveTowards(entity.GetPosition(), targetPosition, speed);

            var coveredDistance = Mathf.Abs(elevatorSystemStateComponent.ElevatorBasePosition.y - entity.GetPosition().y);
            elevatorSystemStateComponent.ElevatorProgress = coveredDistance / Mathf.Abs(elevatorSystemStateComponent.ElevatorBasePosition.y - targetPosition.y);
        }
    }
}