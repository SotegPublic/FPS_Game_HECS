using BluePrints.Identifiers;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionConfig", menuName = "GD/Configs/MissionConfig")]
public class MissionConfig : ScriptableObject
{
    [SerializeField] private Sprite missionImage;
    [SerializeField][IdentifierDropDown(nameof(MissionIdentifier))] private int missionId;
    [SerializeField][IdentifierDropDown(nameof(GradeIdentifier))] private int missionGrade;
    [SerializeField][IdentifierDropDown(nameof(MissionTypeIdentifier))] private int missionType;
    [SerializeField] private int missionCost;
    [SerializeField][EntityContainerIDDropDown(nameof(MissionRewardTagComponent))] private int rewardID;
    [SerializeField] private bool isScenarioMission;
    [SerializeField][IdentifierDropDown(nameof(ShootingZoneIdentifier))] private int startZone;
    [SerializeField] private ZoneScenario[] spawnSteps = new ZoneScenario[0];
    [SerializeField][IdentifierDropDown(nameof(ShootingZoneIdentifier))] private int[] pathNodes;
    [SerializeField][EntityContainerIDDropDown(nameof(LairChestLootVariantTagComponent))][ShowIf("IsScenarioWithChestReward")] private int chestDropID;
    [SerializeField][ShowIf("IsScenarioWithBoss")] private ScenarioBossConfig bossConfig;
    [SerializeField] private MissionConfigForSurvivors configForSurvivors;

    private bool IsScenarioWithBoss => missionType == MissionTypeIdentifierMap.Boss;
    public bool IsScenarioWithChestReward => missionType == MissionTypeIdentifierMap.Chest;

    public Sprite MissionImage => missionImage;
    public int MissionId => missionId;
    public int MissionGrade => missionGrade;
    public int MissionType => missionType;
    public int RewardID => rewardID;
    public int MissionCost => missionCost;
    public bool IsScenarioMission => isScenarioMission;
    public int StartZone => startZone;
    public int ChestDropID => chestDropID;
    public ScenarioBossConfig BossConfig => bossConfig;
    public int[] PathNodes => pathNodes;
    public ZoneScenario[] SpawnSteps => spawnSteps;
    public MissionConfigForSurvivors ConfigForSurvivors => configForSurvivors;
}

[Serializable]
public struct ScenarioBossConfig
{
    [SerializeField] private EntityContainer scenarioBossContainer;
    [SerializeField][EntityContainerIDDropDown(nameof(EnemyLootVariantTagComponent))] private int lootDropID;

    public EntityContainer ScenarioBossContainer => scenarioBossContainer;
    public int LootDropID => lootDropID;
}

[Serializable]
public struct StepActions
{
    public int Step;
    public AsyncActionBluePrint[] NextZonesIDs;
}

[Serializable]
public class MissionConfigForSurvivors
{
    [SerializeField] private int requiredSurvivors;
    [SerializeField] private long durationInSeconds;

    public int RequiredSurvivors => requiredSurvivors;
    public long DurationInSeconds => durationInSeconds;
}