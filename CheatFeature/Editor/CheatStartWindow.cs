using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Helpers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.GridLayoutGroup;


[InitializeOnLoad]
public class CheatStartWindow : OdinEditorWindow
{
    [Header("Weapons")]
    [ValueDropdown(nameof(GetWeaponContainers))]
    [SerializeField]
    [MaxCount(6)]
    private EntityContainer[] weapons;

    [Header("Artefacts")]
    [ValueDropdown(nameof(GetArtefactContainers))]
    [SerializeField]
    [MaxCount(4)]
    private EntityContainer[] artefacts;

    [SerializeField]
    private bool isScenarioOn;

    [SerializeField]
    [ShowIf("isScenarioOn")]
    [ValueDropdown(nameof(GetScenarioMission))]
    private MissionConfig scenarioMissionConfig;

    [SerializeField]
    [HideIf("isScenarioOn")]
    [ValueDropdown(nameof(GetMission))]
    private MissionConfig missionConfig;

    [SerializeField]
    [ValueDropdown(nameof(GetShootingZones))]
    private ShootingZoneIdentifier targetZoneIndex;

    private string errorMessage;

    public static void OpenConfiguratorWindow()
    {
        GetWindow<CheatStartWindow>().Show();
    }

    [Button("Open Shelter", ButtonSizes.Large)]
    private void OpenShelter()
    {
        var world = EntityManager.Worlds.FirstOrDefault();
        var cheatFeature = world.GetEntityBySingleComponent<CheatFeatureTagComponent>();
        var cheatStartComponent = cheatFeature.GetOrAddComponent<CheatStartParametersComponent>();

        cheatStartComponent.IsOpenShelter = true;

        var state = world.GetSingleComponent<GameStateComponent>().CurrentState;
        if (state == GameStateIdentifierMap.ShelterModeInProgress)
            world.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.CheatUnloadShelterSceneState });
        else
            world.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.CheatCleanRaidState });
    }

    [Button("Restart", ButtonSizes.Large)]
    private void Restart()
    {
        if (!CanRestart())
        {
            EditorUtility.DisplayDialog("Restart error", errorMessage, "Ok");
            return;
        }

        var world = EntityManager.Worlds.FirstOrDefault();
        var cheatFeature = world.GetEntityBySingleComponent<CheatFeatureTagComponent>();
        var cheatStartComponent = cheatFeature.GetOrAddComponent<CheatStartParametersComponent>();

        cheatStartComponent.Weapons = weapons;
        cheatStartComponent.Artefacts = artefacts;
        cheatStartComponent.MissionID = isScenarioOn ? scenarioMissionConfig.MissionId : missionConfig.MissionId;
        cheatStartComponent.StartZoneIndex = isScenarioOn ? scenarioMissionConfig.StartZone : missionConfig.StartZone;
        cheatStartComponent.TargetZoneIndex = targetZoneIndex.Id;
        cheatStartComponent.IsScenarioOn = isScenarioOn;
        cheatStartComponent.IsOpenShelter = false;

        cheatFeature.AddComponent<CheatStartTagComponent>();

        var state = world.GetSingleComponent<GameStateComponent>().CurrentState;
        if(state == GameStateIdentifierMap.ShelterModeInProgress)
            world.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.CheatUnloadShelterSceneState });
        else
            world.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.CheatCleanRaidState });
    }

    private bool CanRestart()
    {
        errorMessage = "";

        var isWeaponSet = weapons.Length > 0;

        if (!isWeaponSet)
            errorMessage = "Не выбрано оружие\n";

        if (isScenarioOn)
        {
            var isScenarioParametersSelected = scenarioMissionConfig != null && targetZoneIndex != null;

            if (!isScenarioParametersSelected)
                errorMessage = errorMessage + "Для сценария необходимо выбрать: сценарий, стартовую и целевую зоны\n";

            return isScenarioParametersSelected && isWeaponSet;
        }
        else
        {
            var isStartParametersSelected = targetZoneIndex != null && missionConfig != null;

            if (!isStartParametersSelected)
                errorMessage = errorMessage + "Необходимо выбрать: стартовую и целевую зоны\n";

            return isStartParametersSelected && isWeaponSet;
        }
    }

    private IEnumerable<EntityContainer> GetWeaponContainers()
    {
        var containers = new SOProvider<EntityContainer>().GetCollection().Where(x => x is not PresetContainer && x.IsHaveComponent<EquipItemTagComponent>()
           && x.GetComponent<EquipItemTagComponent>().ItemType == LootItemTypeIdentifierMap.Weapon
           && !x.ContainsComponent(ComponentProvider<IgnoreReferenceContainerTagComponent>.TypeIndex, true));

        return containers;
    }

    private IEnumerable<EntityContainer> GetArtefactContainers()
    {
        var containers = new SOProvider<EntityContainer>().GetCollection().Where(x => x is not PresetContainer && x.IsHaveComponent<EquipItemTagComponent>()
           && x.GetComponent<EquipItemTagComponent>().ItemType == LootItemTypeIdentifierMap.Artefact
           && !x.ContainsComponent(ComponentProvider<IgnoreReferenceContainerTagComponent>.TypeIndex, true));

        return containers;
    }

    private IEnumerable<MissionConfig> GetScenarioMission()
    {
        var raidManager = new SOProvider<EntityContainer>().GetCollection().Where(x => x is not PresetContainer && x.IsHaveComponent<RaidManagerTagComponent>()).FirstOrDefault();
        var missionsHolder = raidManager.GetComponent<MissionsConfigsHolderComponent>();

        var scenarioMissions = missionsHolder.ScenarioMissionsOrder
            .Join(
                missionsHolder.Missions,
                orderID => orderID,
                mission => mission.MissionId,
                (orderID, mission) => mission
            );

        return scenarioMissions;
    }

    private IEnumerable<MissionConfig> GetMission()
    {
        var raidManager = new SOProvider<EntityContainer>().GetCollection().Where(x => x is not PresetContainer && x.IsHaveComponent<RaidManagerTagComponent>()).FirstOrDefault();
        var missionsHolder = raidManager.GetComponent<MissionsConfigsHolderComponent>();

        var missions = missionsHolder.Missions.Where(m => !missionsHolder.ScenarioMissionsOrder.Contains(m.MissionId));

        return missions;
    }

    private IEnumerable<ShootingZoneIdentifier> GetShootingZones()
    {
        if ((isScenarioOn && scenarioMissionConfig == null) || (!isScenarioOn && missionConfig == null))
            return Enumerable.Empty<ShootingZoneIdentifier>();

        var config = isScenarioOn ? scenarioMissionConfig : missionConfig;
        var pathNodes = config.PathNodes;

        var identifiersCollection = new ShootingZoneIdentifier[pathNodes.Length];
        var identifiers = new SOProvider<ShootingZoneIdentifier>().GetCollection().ToArray();

        var collectionIndex = 0;

        foreach (var node in pathNodes)
        {
            var identifier = identifiers.FirstOrDefault(i => i.Id == node);

            if(identifier != null)
                identifiersCollection[collectionIndex++] = identifier;
        }

        return identifiersCollection;
    }
}