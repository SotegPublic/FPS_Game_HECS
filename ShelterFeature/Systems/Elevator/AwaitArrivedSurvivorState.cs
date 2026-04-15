using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "AwaitArrivedSurvivorState")]
    public class AwaitArrivedSurvivorState : ShelterElevatorBaseState
    {
        private ArrivedSurvivorsHolderComponent arrivedSurvivorsHolderComponent;
        private PassengerHolderComponent passengerHolderComponent;
        private ElevatorConfigComponent elevatorConfig;

        public AwaitArrivedSurvivorState(StateMachine stateMachine, int nextDefaultState,
            ArrivedSurvivorsHolderComponent arrivedSurvivorsHolder, PassengerHolderComponent passengerHolder, ElevatorConfigComponent configComponent) : base(stateMachine, nextDefaultState)
        {
            arrivedSurvivorsHolderComponent = arrivedSurvivorsHolder;
            passengerHolderComponent = passengerHolder;
            elevatorConfig = configComponent;
        }

        public override int StateID => ShelterElevatorBaseState.AwaitArrivedSurvivorState;

        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            foreach(var arrivedSurvivor in arrivedSurvivorsHolderComponent.Survivors)
            {
                var room = arrivedSurvivor.Key;
                foreach(var survivor in arrivedSurvivor.Value)
                {
                    if (survivor.IsInBunker)
                    {
                        passengerHolderComponent.Passenger = survivor;
                        passengerHolderComponent.TargetRoom = room;
                        var elevatorPosition = entity.GetPosition();
                        elevatorPosition.y += elevatorConfig.ElevatorFloorOffset;

                        survivor.Entity.GetTransform().position = elevatorPosition;
                        EndState();
                        return;
                    }
                }
            }
        }
    }
}