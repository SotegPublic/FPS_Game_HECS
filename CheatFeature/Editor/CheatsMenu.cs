using Commands;
using Components;
using HECSFramework.Core;
using System.Linq;
using UnityEditor;

public class CheatsMenu
{
    private const string ItemPath = "StalkerSurvivour/Cheats/";

    [MenuItem(ItemPath + "Kill Them All &k")] // Alt + K
    public static void DestroyEnemies()
    {
        var world = EntityManager.Worlds.FirstOrDefault();

        foreach(var enemy in world.GetFilter<EnemyTagComponent>())
        {
            world.Command(new DestroyEntityWorldCommand { Entity = enemy });
        }
    }

    [MenuItem(ItemPath + "Move To Destination &m")] // Alt + M
    public static void MoveMainCharacterToDestination()
    {
        var world = EntityManager.Worlds.FirstOrDefault();

        var mainchar = world.GetEntityBySingleComponent<MainCharacterTagComponent>();

        var agent = mainchar.GetComponent<NavMeshAgentComponent>().NavMeshAgent;
        var destination = agent.destination;
        var direction = (destination - mainchar.GetTransform().position).normalized;
        var offsetDistance = agent.stoppingDistance * 50;

        mainchar.GetTransform().position = destination - direction * offsetDistance;
    }

    [MenuItem(ItemPath + "Shield Parry Ability #1")] // Shift + 1
    public static void ShieldParryAbility()
    {
        var world = EntityManager.Worlds.FirstOrDefault();
        world.Command(new AbilityButtonCommand { AbilityIndex = AdditionalAbilityIdentifierMap.Slot1 });
    }

    [MenuItem(ItemPath + "Shield Block Ability #2")] // Shift + 2
    public static void ShieldBlockAbility()
    {
        var world = EntityManager.Worlds.FirstOrDefault();
        world.Command(new AbilityButtonCommand { AbilityIndex = AdditionalAbilityIdentifierMap.Slot2 });
    }

    [MenuItem(ItemPath + "Cheat Start Window &T")] // Alt + T
    public static void OpenWindow()
    {
        CheatStartWindow.OpenConfiguratorWindow();
    }

    [MenuItem(ItemPath + "Strafe To Left #l")] // Shift + L
    public static void StrafeToLeft()
    {
        var world = EntityManager.Worlds.FirstOrDefault();
        world.Command(new StrafeToLeftCommand());
    }

    [MenuItem(ItemPath + "Strafe To Right #r")] // Shift + R
    public static void StrafeToRight()
    {
        var world = EntityManager.Worlds.FirstOrDefault();
        world.Command(new StrafeToRightCommand());
    }
}
