using Commands;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ProcessClickState")]
    public class EndMoveState : ShelterControlBaseState
    {
        public EndMoveState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.EndMoveState;


        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            entity.World.Command(new StopMoveShelterCameraCommand());
            EndState();
        }
    }

}
