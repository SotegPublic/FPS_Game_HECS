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
    [Serializable]
    [Documentation(Doc.Loot, "we use this component for configurate chest reward parameters")]
    public sealed class ChestRewardsConfigComponent : BaseComponent
    {
        [SerializeField] private ChestRewardsConfig chestRewardsConfig;

        public ChestRewardsConfig ChestRewardsConfig => chestRewardsConfig;

        public void InitConfig()
        {
            base.Init();
            chestRewardsConfig.InitConfigParameters();
        }
    }

    [Serializable]
    public class ChestRewardsConfig : BaseDropConfig<ChestRandomRewardConfig>
    {
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(0)]
        [Header("Guaranteed drop")]
        [SerializeField][EntityContainerIDDropDown(nameof(EquipItemRewardTagComponent))] private int[] guaranteedRewards;

        public override int[] GuaranteedRewards => guaranteedRewards;

        public override string GetConfigName()
        {
            return "Config parameters";
        }
    }

    [Serializable]
    public class ChestRandomRewardConfig : BaseRandomRewardConfig
    {
        [SerializeField][EntityContainerIDDropDown(nameof(EquipItemRewardTagComponent))] private int randomReward;
        public override int RandomReward => randomReward;
    }
}