using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ClickOrMoveState")]
    public class ClickOrMoveState : ShelterControlBaseState
    {
        public ClickOrMoveState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.ClickOrMoveState;


        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            stateMachine.TryToNextStateByTransition();
            
            if (!stateComponent.IsPressed)
                EndState();
        }
    }

    public class IsMove : ITransition
    {
        ShelterControlSystemStateComponent state;
        private float dragLenght = 1;

        public IsMove(int toState, ShelterControlSystemStateComponent systemState)
        {
            ToState = toState;
            state = systemState;
        }

        public int ToState { get; }

        public bool IsReady()
        {
            return state.DragLenght() >= dragLenght;
        }
    }

}
