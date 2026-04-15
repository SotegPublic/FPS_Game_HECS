using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "MoveSurvivorToRoomState")]
    public class MoveSurvivorToRoomState : ShelterElevatorBaseState
    {
        private PassengerHolderComponent passengerHolderComponent;
        private ElevatorConfigComponent configComponent;

        private float currentAwait;

        public MoveSurvivorToRoomState(StateMachine stateMachine, int nextDefaultState,
            PassengerHolderComponent passengerHolder, ElevatorConfigComponent config) : base(stateMachine, nextDefaultState)
        {
            passengerHolderComponent = passengerHolder;
            configComponent = config;
        }

        public override int StateID => ShelterElevatorBaseState.MoveSurvivorToRoomState;

        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
            currentAwait = 0f;
        }

        public override void Update(Entity entity)
        {
            currentAwait += Time.deltaTime;

            if(currentAwait >= configComponent.AwaitTime)
            {
                var passengerTransform = passengerHolderComponent.Passenger.Entity.GetTransform();
                passengerTransform.position = passengerHolderComponent.TargetRoom.GetComponent<RoomMonocomponentProviderComponent>().Get.FloorEdge.position;
                passengerTransform.parent = passengerHolderComponent.TargetRoom.GetTransform();
                entity.World.Command(new SurvivorArrivedInRoomCommand { Room = passengerHolderComponent.TargetRoom, Survivor = passengerHolderComponent.Passenger });
                EndState();
            }
        }
    }
}