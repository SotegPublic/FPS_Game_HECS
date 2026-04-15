using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine.AddressableAssets;

namespace Systems
{
    [Serializable]
    [Documentation(DocFeature.Raid, Doc.GameState, "this system unload current location and pfbs of zones after cheat command")]
    public sealed class CheatUnLoadRaidZoneStateSystem : BaseGameStateSystem
    {
        [Single] public AssetService AssetService;
        [Required] public CheatStartParametersComponent StartParametersComponent;

        protected override int State { get; } = GameStateIdentifierMap.CheatUnloadSceneState;

        public override void InitSystem()
        {
        }

        protected override async void ProcessState(int from, int to)
        {
            var shooterZoneStateComponent = Owner.World.GetEntityBySingleComponent<ShootingZoneTagComponent>().GetComponent<ShooterZoneStateComponent>();

            if(shooterZoneStateComponent.BattleScene.Scene.isLoaded)
            {
                await Addressables.UnloadSceneAsync(shooterZoneStateComponent.BattleScene).Task;
            }

            shooterZoneStateComponent.Reset();

            if (StartParametersComponent.IsOpenShelter)
                Owner.World.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.LoadShelterSceneState });
            else
                EndState();
        }
    }
}