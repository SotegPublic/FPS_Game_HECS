using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Loot, "we use this component for configurate drop parameters")]
    public sealed class LootDropConfigsComponent : BaseComponent
    {
        [SerializeField] private DropConfig[] dropGradeConfigs;

        public void InitConfigs()
        {
            base.Init();
            for (int i = 0; i < dropGradeConfigs.Length; i++)
            {
                dropGradeConfigs[i].InitConfigParameters();
            }
        }

        public bool TryGetDropConfigByGrade(int grade, out DropConfig config)
        {
            for (int i = 0; i < dropGradeConfigs.Length; i++)
            {
                if (dropGradeConfigs[i].LootGrade == grade)
                {
                    config = dropGradeConfigs[i];
                    return true;
                }
            }

            config = null;
            return false;
        }
    }

    [Serializable]
    public class DropConfig : BaseDropConfig<RandomRewardConfig>
    {
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(0)]
        [SerializeField][IdentifierDropDown(nameof(GradeIdentifier))] private int lootGrade;

        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(0)]
        [Header("Guaranteed drop")]
        [SerializeField][EntityContainerIDDropDown(nameof(DropFromEnemiesRewardTagComponent))] private int[] guaranteedRewards;

        public override int[] GuaranteedRewards => guaranteedRewards;
        public int LootGrade => lootGrade;

        public override string GetConfigName()
        {
            return lootGrade == 0 ? "Grade not set" : IdentifierToStringMap.IntToString[lootGrade];
        }
    }

    [Serializable]
    public class RandomRewardConfig : BaseRandomRewardConfig
    {
        [SerializeField][EntityContainerIDDropDown(nameof(DropFromEnemiesRewardTagComponent))] private int randomReward;
        public override int RandomReward => randomReward;
    }
}