using System;
using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;

namespace Systems
{
    [Serializable]
    [Documentation(DocFeature.Raid, Doc.State, "here we clean level after cheat command from entities and make clean up moves, show loading screen")]
    public sealed class CheatCleanRaidStateSystem : BaseGameStateSystem
    {
        [Required] private CheatStartParametersComponent startParameters;

        protected override int State { get; } = GameStateIdentifierMap.CheatCleanRaidState;

        private EntitiesFilter filter;

        public override void InitSystem()
        {
            filter = Owner.World.GetFilter<CleanLevelTagComponent>();
        }

        protected override async void ProcessState(int from, int to)
        {
            var states = Owner.World.GetSingleComponent<CoreFSMsComponent>();
            states.Shooter.ChangeState(0);
            states.SpawnEnemies.ChangeState(ZoneSpawnState.WaitForSpawn);

            Owner.World.Command(new ShowAimCommand { IsAimShowing = false });
            Owner.World.Command(new UIGroupCommand { Show = false, UIGroup = UIGroupIdentifierMap.ShooterUIGroupIdentifier });
            await Owner.World.GetSingleSystem<UISystem>().ShowUI(UIIdentifierMap.LoadingScreen_UIIdentifier);

            Owner.World.Command(new CleanRaidGlobalCommand());

            if(Owner.World.TryGetEntityBySingleComponent<MainCharacterTagComponent>(out var character))
            {
                character.Command(new RemoveAllBuffs());
                character.HecsDestroy();
            }

            filter.ForceUpdateFilter();

            foreach (var e in filter)
                e.HecsDestroy();

            await UniTask.DelayFrame(1);

            EndState();
        }
    }
}

