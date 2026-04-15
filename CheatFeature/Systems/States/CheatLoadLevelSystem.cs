using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

namespace Systems
{
	[Serializable][Documentation(Doc.State, Doc.Level, "this system spawn level")]
    public sealed class CheatLoadLevelSystem : BaseGameStateSystem
    {
        [Required] private CheatStartParametersComponent startParameters;
        [Single]
        public AssetService AssetService;

        protected override int State { get; } = GameStateIdentifierMap.CheatLoadZone;

        private int FirstEntrance = IndexGenerator.GenerateIndex("FirstEntrance");

        public override void InitSystem()
        {
        }

        protected async override void ProcessState(int from, int to)
        {
            var shootingZoneFeature = Owner.World.GetEntityBySingleComponent<ShootingZoneTagComponent>();
            var shooterZoneStateComponent = shootingZoneFeature.GetComponent<ShooterZoneStateComponent>();
            var zonesGlobalHolder = shootingZoneFeature.GetComponent<ZonesGlobalHolderComponent>();

            shooterZoneStateComponent.MissionID = startParameters.MissionID;
            shooterZoneStateComponent.EnterID = startParameters.StartZoneIndex;

            var sceneTuple = zonesGlobalHolder.GetZoneByEnterID(startParameters.StartZoneIndex);
            shooterZoneStateComponent.BattleScene = await Addressables.LoadSceneAsync(sceneTuple.scene, LoadSceneMode.Additive).Task;
            SceneManager.SetActiveScene(shooterZoneStateComponent.BattleScene.Scene);

            if (sceneTuple.zonePref != null)
                await AssetService.GetAssetInstance(sceneTuple.zonePref);

            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var progress = player.GetComponent<ShooterPlayerProgressComponent>();

            if (startParameters.IsScenarioOn)
            {
                var missionsConfigs = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>().GetComponent<MissionsConfigsHolderComponent>();
                
                for(int i = 0; i < progress.MaxScenarioIndex; i++)
                {
                    var missionID = missionsConfigs.GetScenarioMissionID(i);

                    if(missionID == startParameters.MissionID)
                    {
                        progress.CurrentScenarioIndex = i;
                        break;
                    }
                }
            }
            else
            {
                progress.CurrentScenarioIndex = int.MaxValue;
            }

            shootingZoneFeature.Command(new MissionZoneLoadedCommand());

            EndState();
        }
    }
}