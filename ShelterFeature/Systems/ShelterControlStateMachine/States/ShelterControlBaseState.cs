using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "StartProcessTouch")]
    public abstract class ShelterControlBaseState : BaseFSMState
    {
        public readonly static int AwaitTouchState = IndexGenerator.GenerateIndex(nameof(AwaitTouchState));
        public readonly static int ClickOrMoveState = IndexGenerator.GenerateIndex(nameof(ClickOrMoveState));
        public readonly static int ProcessClickState = IndexGenerator.GenerateIndex(nameof(ProcessClickState));
        public readonly static int StartMoveState = IndexGenerator.GenerateIndex(nameof(StartMoveState));
        public readonly static int MoveInProgressState = IndexGenerator.GenerateIndex(nameof(MoveInProgressState));
        public readonly static int EndMoveState = IndexGenerator.GenerateIndex(nameof(EndMoveState));


        protected ShelterControlSystemStateComponent stateComponent;

        protected ShelterControlBaseState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState)
        {
            stateComponent = owner.GetComponent<ShelterControlSystemStateComponent>();
        }
    }

}
