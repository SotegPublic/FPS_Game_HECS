using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ShelterElevatorBaseState")]
    public abstract class ShelterElevatorBaseState : BaseFSMState
    {
        public readonly static int AwaitArrivedSurvivorState = IndexGenerator.GenerateIndex(nameof(AwaitArrivedSurvivorState));
        public readonly static int GoToTargetRoomState = IndexGenerator.GenerateIndex(nameof(GoToTargetRoomState));
        public readonly static int MoveSurvivorToRoomState = IndexGenerator.GenerateIndex(nameof(MoveSurvivorToRoomState));
        public readonly static int ElevatorReturnState = IndexGenerator.GenerateIndex(nameof(ElevatorReturnState));
        public readonly static int ResetElevatorState = IndexGenerator.GenerateIndex(nameof(ResetElevatorState));

        protected ShelterElevatorBaseState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
        {
        }
    }
}