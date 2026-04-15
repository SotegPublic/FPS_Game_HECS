using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Config, Doc.Loot, Doc.Lair, "here we configurate chests loot")]
    public sealed class ChestsLootConfigsComponent : BaseComponent
    {
        [SerializeField] private ChestLootConfig[] configs;

        public bool TryGetChestConfigByID(int id, out ChestLootConfig config)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                if (configs[i].ZoneID == id)
                {
                    config = configs[i];
                    return true;
                }
            }

            config = default;
            return false;
        }
    }

    [Serializable]
    public struct ChestLootConfig
    {
        [IdentifierDropDown(nameof(ShootingZoneIdentifier))] public int ZoneID;
        [EntityContainerIDDropDown(nameof(LairChestLootVariantTagComponent))] public int LootDropID;
    }
}