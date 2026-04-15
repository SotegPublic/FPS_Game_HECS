using Commands;
using Components;
using Systems;

namespace HECSFramework.Core.ShooterMiniGameStates
{
    [Documentation(Doc.ShooterFeature, "this state start game raid")]
    public class StartRaidState : ShooterState
    {
        public override int StateID { get; } = ShooterState.StartRaidState;

        public StartRaidState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
        {
        }

        public override void Enter(Entity entity)
        {
            entity.World.Command(new CheckQuestsGlobalCommand());
            EndState();
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
        }
    }

    public class IsCheatStart : ITransition
    {
        private World world;

        public IsCheatStart(World world, int toState)
        {
            this.world = world;
            ToState = toState;
        }

        public int ToState { get; }

        public bool IsReady()
        {
            var cheatFeature = world.GetEntityBySingleComponent<CheatFeatureTagComponent>();

            return cheatFeature.ContainsMask<CheatStartTagComponent>();
        }
    }
}