using Commands;
using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.Shelter, Doc.State, "ProcessClickState")]
    public class ProcessClickState : ShelterControlBaseState
    {
        public ProcessClickState(StateMachine stateMachine, int nextDefaultState, Entity owner) : base(stateMachine, nextDefaultState, owner)
        {
        }

        public override int StateID { get; } = ShelterControlBaseState.ProcessClickState;


        public override void Enter(Entity entity)
        {
            //if (entity.World.TryGetSingleComponent(out InputOverUIComponent inputOverUIComponent))
            //{
            //    if (inputOverUIComponent.InputOverUI(stateComponent.From))
            //    {
            //        // if we need process click on some UI
            //    }
            //}

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
