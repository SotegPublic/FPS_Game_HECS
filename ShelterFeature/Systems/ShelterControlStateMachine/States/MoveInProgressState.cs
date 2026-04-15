using Commands;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ProcessClickState")]
    public class MoveInProgressState : ShelterControlBaseState
    {
        public MoveInProgressState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.MoveInProgressState;


        public override void Enter(Entity entity)
        {
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
            entity.World.Command(new MoveShelterCameraCommand { CurrentPointerPosition = stateComponent.CurrentCoord });

            if (!stateComponent.IsPressed)
                EndState();
        }
    }

}
