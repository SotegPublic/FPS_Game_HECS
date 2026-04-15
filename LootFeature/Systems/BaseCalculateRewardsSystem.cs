using BluePrints.Identifiers;
using Components;
using HECSFramework.Core;
using Helpers;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static Systems.BaseCalculateRewardsSystem;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Loot, Doc.Rewards, "base system for calculate rewards")]
    public abstract class BaseCalculateRewardsSystem : BaseSystem
    {
        [HECSFramework.Core.Required] public LootRewardsHolderComponent RewardsHolder;

        protected void CalculateAndAddRandomRewards(IDropConfig dropConfig)
        {
            var repeatCount = dropConfig.IsBetweenValues ? GetRepeatValue(dropConfig) : dropConfig.DropsCount;

            if (dropConfig.IsUniqueDrops)
            {
                if (repeatCount >= dropConfig.RandomRewards.Length)
                {
                    GetAllRewards(dropConfig);
                }
                else
                {
                    var startMaxWeightPrefix = dropConfig.MaxWeightPrefix;
                    PrepareAvailableRewardsList(dropConfig);

                    for (int i = 0; i < repeatCount; i++)
                    {
                        var rewardIndex = GetUniqueRandomReward(dropConfig);
                        if(rewardIndex != -1)
                            RewardsHolder.Rewards.Add(rewardIndex);
                    }

                    dropConfig.MaxWeightPrefix = startMaxWeightPrefix;
                }
            }
            else
            {
                var dropsCount = repeatCount > dropConfig.RandomRewards.Length ? dropConfig.RandomRewards.Length : repeatCount;

                for (int i = 0; i < dropsCount; i++)
                {
                    var reward = GetRandomReward(dropConfig);
                    if (reward != 0)
                    {
                        RewardsHolder.Rewards.Add(reward);
                    }
                }
            }
        }

        private void GetAllRewards(IDropConfig dropConfig)
        {
            for (int i = 0; i < dropConfig.RandomRewards.Length; i++)
            {
                RewardsHolder.Rewards.Add(dropConfig.RandomRewards[i].RandomReward);
            }
        }

        private void PrepareAvailableRewardsList(IDropConfig dropConfig)
        {
            dropConfig.AvailableRewards.ClearFast();

            for (int i = 0; i < dropConfig.RandomRewards.Length; i++)
            {
                dropConfig.AvailableRewards.Add(dropConfig.RandomRewards[i]);
            }
        }

        private int GetUniqueRandomReward(IDropConfig dropConfig)
        {
            var randomWeight = UnityEngine.Random.Range(0, dropConfig.MaxWeightPrefix);
            var currentWeight = 0;

            for (int i = 0; i < dropConfig.AvailableRewards.Count; i++)
            {
                currentWeight += dropConfig.AvailableRewards[i].DropWeight;
                if (currentWeight >= randomWeight)
                {
                    dropConfig.MaxWeightPrefix -= dropConfig.AvailableRewards[i].DropWeight;
                    var reward = dropConfig.AvailableRewards[i].RandomReward;
                    dropConfig.AvailableRewards.RemoveAtSwap(i);
                    return reward;
                }
            }

            return -1;
        }

        private int GetRandomReward(IDropConfig dropConfig)
        {
            var randomWeight = UnityEngine.Random.Range(0, dropConfig.MaxWeightPrefix);

            int left = 0;
            int right = dropConfig.PrefixSums.Length - 1;

            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (randomWeight < dropConfig.PrefixSums[mid])
                    right = mid;
                else
                    left = mid + 1;
            }

            return dropConfig.RandomRewards[left].RandomReward;
        }

        private int GetRepeatValue(IDropConfig dropConfig)
        {
            return UnityEngine.Random.Range(dropConfig.MinDropsCount, dropConfig.MaxDropsCount);
        }

        public interface IDropConfig
        {
            public int MaxWeightPrefix { get; set; }
            public int[] GuaranteedRewards { get; }
            public IRandomRewardConfig[] RandomRewards { get; }
            public bool IsBetweenValues { get; }
            public int DropsCount { get; }
            public int MinDropsCount { get; }
            public int MaxDropsCount { get; }
            public bool IsUniqueDrops { get; }
            public int[] PrefixSums { get; }
            public HECSList<IRandomRewardConfig> AvailableRewards { get; }
        }

        public interface IRandomRewardConfig
        {
            public int DropWeight { get; }
            public int RandomReward { get; }
        }
    }

    public abstract class BaseDropConfig<TRandomReward> : IDropConfig where TRandomReward : BaseRandomRewardConfig, new()
    {
        [FoldoutGroup("@GetConfigName()")]
        [Header("Random drop")]
        [PropertyOrder(1)]
        [SerializeReference] private TRandomReward[] randomRewards;
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(1)]
        [SerializeField] private bool isBetweenValues;
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(1)]
        [SerializeField][HideIf("isBetweenValues")] private int dropsCount = 1;
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(1)]
        [SerializeField][ShowIf("isBetweenValues")] private int minRandomDropsCount = 1;
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(1)]
        [SerializeField][ShowIf("isBetweenValues")] private int maxRandomDropsCount = 2;
        [FoldoutGroup("@GetConfigName()")]
        [PropertyOrder(1)]
        [SerializeField] private bool isUniqueDrops;

        private int[] prefixSums;
        private HECSList<IRandomRewardConfig> availableRewards;
        [HideInInspector][NonSerialized] private int maxWeightPrefix;

        public abstract string GetConfigName();
        public int MaxWeightPrefix { get => maxWeightPrefix; set => maxWeightPrefix = value; }
        public abstract int[] GuaranteedRewards { get; }
        public IRandomRewardConfig[] RandomRewards => randomRewards;
        public bool IsBetweenValues => isBetweenValues;
        public int DropsCount => dropsCount;
        public int MinDropsCount => minRandomDropsCount;
        public int MaxDropsCount => maxRandomDropsCount;
        public bool IsUniqueDrops => isUniqueDrops;
        public int[] PrefixSums => prefixSums;
        public HECSList<IRandomRewardConfig> AvailableRewards => availableRewards;

        public void InitConfigParameters()
        {
            prefixSums = new int[randomRewards.Length];
            maxWeightPrefix = 0;

            for (int i = 0; i < randomRewards.Length; i++)
            {
                maxWeightPrefix += randomRewards[i].DropWeight;
                prefixSums[i] = maxWeightPrefix;
            }

            if (isUniqueDrops)
                availableRewards = new HECSList<IRandomRewardConfig>(randomRewards.Length);
        }
    }

    public abstract class BaseRandomRewardConfig : IRandomRewardConfig
    {
        [SerializeField][Range(0, 100)] protected int dropWeight;

        public int DropWeight => dropWeight;
        public abstract int RandomReward { get;  }
    }
}