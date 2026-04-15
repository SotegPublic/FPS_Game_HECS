using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Missions, Doc.MissionsGeneration, "this system generate mission")]
    public sealed class GenerateMissionSystem : BaseSystem, IReactGlobalCommand<GenerateMissionCommand>
    {
        [Required] public MissionsConfigsHolderComponent MissionsByEnter;
        [Required] public MissionsDifficultyConfigComponent DifficultyConfig;
        [Required] public RaidScenarioComponent RaidScenarioComponent;
        [Required] public MissionsGeneratorConfigComponent GeneratorConfig;
        [Required] public PathTemplatesHolderComponent PathTemplates;
        [Required] public SpawnTemplatesHolderComponent SpawnTemplates;

        public void CommandGlobalReact(GenerateMissionCommand command)
        {
            var raidManager = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            var missionConfig = raidManager.GetComponent<MissionsConfigsHolderComponent>().GetMissionByID(command.MissionID);

            RaidScenarioComponent.StepNodes = GetStepNodes(missionConfig);
            RaidScenarioComponent.RaidSteps = GenerateRaid(missionConfig);

            RaidScenarioComponent.RaidGrade = missionConfig.MissionGrade;


            if (missionConfig.MissionType == MissionTypeIdentifierMap.Boss)
                RaidScenarioComponent.BossConfig = GetBossConfig(missionConfig);

            if (missionConfig.MissionType == MissionTypeIdentifierMap.Chest)
                RaidScenarioComponent.ChestDropID = GetChestDropID(missionConfig);
        }

        private ScenarioBossConfig GetBossConfig(MissionConfig missionConfig)
        {
            //todo - add logic for randomize bosses
            return missionConfig.BossConfig;
        }

        private int GetChestDropID(MissionConfig missionConfig)
        {
            //todo - add logic for randomize chests
            if(missionConfig.ChestDropID != 0)
                return missionConfig.ChestDropID;

            var chestLootConfigs = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<ChestsLootConfigsComponent>();
            var lastIndex = missionConfig.PathNodes.Length - 1;
            if (chestLootConfigs.TryGetChestConfigByID(missionConfig.PathNodes[lastIndex], out var enemyLootConfig))
                return enemyLootConfig.LootDropID;

            return 0;
        }

        private int[] GetStepNodes(MissionConfig missionConfig)
        {
            if(missionConfig.PathNodes != null && missionConfig.PathNodes.Length > 0)
            {
                return missionConfig.PathNodes;
            }
            else
            {
                var enter = missionConfig.StartZone;
                var type = missionConfig.MissionType;
                if(PathTemplates.TryGetRandomPathTemplate(enter, type, out var template))
                {
                    return template.Path;
                }
            }

            throw new Exception("no path in misson error");
        }

        private ChoiceStep[] GenerateRaid(MissionConfig config)
        {
            var spawnSteps = config.SpawnSteps;
            if (spawnSteps != null && spawnSteps.Length > 0)
            {
                return GetSpawns(spawnSteps);
            }
            else
            {
                var stepsCount = RaidScenarioComponent.StepNodes.Length;
                var steps = new ChoiceStep[stepsCount];
                if (config.MissionType == MissionTypeIdentifierMap.Exploration ||
                    config.MissionType == MissionTypeIdentifierMap.Boss)
                    stepsCount--;

                if (SpawnTemplates.TryGetRandomSpawnsTemplate(config.MissionGrade, stepsCount, out var template))
                {
                    for(int i = 0; i < stepsCount; i++)
                    {
                        steps[i] = new ChoiceStep();
                        steps[i].ZoneScenario = template.Spawns[i];
                    }

                    return steps;
                }
                else
                {
                    for (int i = 0; i < stepsCount; i++)
                    {
                        steps[i] = new ChoiceStep();
                        steps[i].ZoneScenario = GenerateZoneScenario(config);
                    }

                    return steps;
                }
            }
        }

        private static ChoiceStep[] GetSpawns(ZoneScenario[] spawnSteps)
        {
            var stepsCount = spawnSteps.Length;
            var steps = new ChoiceStep[stepsCount];

            for (int i = 0; i < stepsCount; i++)
            {
                steps[i] = new ChoiceStep();
                steps[i].ZoneScenario = spawnSteps[i];
            }

            return steps;
        }

        private ZoneScenario GenerateZoneScenario(MissionConfig config)
        {
            var waves = UnityEngine.Random.Range(0, GeneratorConfig.WavesRandom + 1) + GeneratorConfig.BaseWavesCount;

            var zoneScenario = new ZoneScenario();
            zoneScenario.ZoneWaves = new ZoneWave[waves];

            for (int i = 0; i < waves; i++)
            {
                var delay = UnityEngine.Random.Range(0, GeneratorConfig.WaitMaxDelay);
                zoneScenario.ZoneWaves[i] = GetWave(i, delay, config);
            }

            return zoneScenario;
        }

        private ZoneWave GetWave(int order, float delay, MissionConfig config)
        {
            var zoneWave = new ZoneWave();
            zoneWave.Order = order;
            zoneWave.Delay = delay;


            var enemiesHolder = Owner.World.GetSingleComponent<EnemiesHolderComponent>();
            var enemiesCount = UnityEngine.Random.Range(GeneratorConfig.MonstersCount.x, GeneratorConfig.MonstersCount.y);

            var currentLvl = Owner.World.GetSingleComponent<ShooterPlayerProgressComponent>().CurrentLvl;
            var difficultyModifier = DifficultyConfig.GetDifficultyValue(config.MissionGrade, config.MissionType, config.StartZone);
            var min = difficultyModifier <= 0 ? difficultyModifier : 0;
            var max = difficultyModifier <= 0 ? 0 : difficultyModifier;

            zoneWave.Enemies = new ZoneEnemy[enemiesCount];

            for (int i = 0; i < enemiesCount; i++)
            {
                int upDatedMonsterlvl = currentLvl + UnityEngine.Random.Range(min, max + 1);
                upDatedMonsterlvl = upDatedMonsterlvl < 0 ? 0 : upDatedMonsterlvl;

                var enemy = enemiesHolder.GetRandomMonsterBy(upDatedMonsterlvl);
                zoneWave.Enemies[i].EnemyID = enemy.ContainerIndex;

                var enemiesLootConfigs = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EnemiesLootConfigsComponent>();
                var grade = config.MissionGrade;

                if (enemiesLootConfigs.TryGetEnemyConfigByID(zoneWave.Enemies[i].EnemyID, out var enemyConfig))
                {
                    zoneWave.Enemies[i].LootDropID = enemyConfig.LootDropID;
                    zoneWave.Enemies[i].LootGradeID = grade;
                }

            }

            return zoneWave;
        }

        public override void InitSystem()
        {
        }
    }
}


namespace Commands
{
    [Documentation(Doc.Missions, "we send this command when we should generate mission for raid and place this scenario to " + nameof(RaidScenarioComponent))]
    public struct GenerateMissionCommand : IGlobalCommand
    {
        public int MissionID;
        public int EnterID;
    }
}