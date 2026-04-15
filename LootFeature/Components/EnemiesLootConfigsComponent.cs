using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Config, Doc.Loot, Doc.Enemy, "here we configurate enemies loot")]
    public sealed class EnemiesLootConfigsComponent : BaseComponent
    {
        [SerializeField] private EnemyLootConfig[] configs;

        public bool TryGetEnemyConfigByID(int id, out EnemyLootConfig config) 
        {
            for(int i = 0; i < configs.Length; i++)
            {
                if (configs[i].EnemyID == id)
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
    public struct EnemyLootConfig
    {
        [EntityContainerIDDropDown(nameof(EnemyTagComponent))] public int EnemyID;
        [EntityContainerIDDropDown(nameof(EnemyLootVariantTagComponent))] public int LootDropID;
    }
}