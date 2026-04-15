using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "AwaitTouch")]
    public class AwaitTouchState : ShelterControlBaseState
    {
        public AwaitTouchState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.AwaitTouchState;


        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            if (stateComponent.IsPressed)
            {
                EndState();
            }
        }
    }

}
