using Commands;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ProcessClickState")]
    public class StartMoveState : ShelterControlBaseState
    {
        public StartMoveState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.StartMoveState;


        public override void Enter(Entity entity)
        {
            entity.World.Command(new StartMoveShelterCameraCommand { StartPosition = stateComponent.From });
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
