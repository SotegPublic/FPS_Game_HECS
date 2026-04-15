using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using HECSFramework.Unity;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Loot, "this system spawn rewards views")]
    public sealed class LootVisualSystem : BaseSystem, IUpdatable, IReactCommand<SpawnRewardsViewsCommand>, IInitAfterView
    {
        [Required] private LootRewardsHolderComponent rewardsHolder;
        [Required] private RewardActorsHolderComponent rewardActorsHolder;
        [Required] private LootVisualConfigComponent visualConfig;

        [Single] public PoolingSystem PoolingSystem;
        [Single] public RewardsGlobalHolderComponent GlobalRewardsHolder;

        private List<UniTask<Actor>> tasks = new List<UniTask<Actor>>();
        private UniTask spawnViewTask;
        private bool isLootViewReady;
        private bool isRewardsViewsReady;

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if (Owner.ContainsMask<LootReadyTagComponent>())
                return;

            if(isLootViewReady && isRewardsViewsReady)
                Owner.AddComponent<LootReadyTagComponent>();
        }

        public void InitAfterView()
        {
            var view = Owner.GetComponent<ViewReadyTagComponent>().View;
            if (view.TryGetComponent<ParticlesProviderMonoComponent>(out var component))
            {
                component.ParticleSystem.Play();
            }

            isLootViewReady = true;
        }

        public void CommandReact(SpawnRewardsViewsCommand command)
        {
            SpawnRewardsViewsAsync().Forget();
        }

        private async UniTask SpawnRewardsViewsAsync()
        {
            for (int i = 0; i < rewardsHolder.Rewards.Count; i++)
            {
                if (GlobalRewardsHolder.TryGetContainerByID(rewardsHolder.Rewards[i], out var container))
                {
                    tasks.Add(GetActor(container));
                }
            }

            var result = await UniTask.WhenAll(tasks);
            await spawnViewTask;

            for (int i = 0; i < result.Length; i++)
            {
                rewardActorsHolder.RewardActors.Add(result[i]);
            }

            isRewardsViewsReady = true;
        }

        private async UniTask<Actor> GetActor(EntityContainer rewardContainer)
        {
            var position = Owner.GetPosition();

            if (rewardContainer.TryGetComponent<RewardWithViewTagComponent>(out var tag))
            {
                position = GetSpawnPosition();
            }

            var actor = await rewardContainer.GetActor(position: position, rotation: Quaternion.identity);
            actor.Init();

            return actor;
        }

        private Vector3 GetSpawnPosition()
        {
            var playerPosition = Owner.World.GetEntityBySingleComponent<PlayerCharacterComponent>().GetPosition();
            var lootCenterPosition = Owner.GetPosition();

            var dir = (playerPosition - lootCenterPosition).normalized;
            var randomAngle = Random.Range(-visualConfig.ArcAngle / 2f, visualConfig.ArcAngle / 2f);
            var rotation = Quaternion.Euler(0, randomAngle, 0);
            var spawnDirection = rotation * dir;

            return lootCenterPosition + spawnDirection * visualConfig.SpawnDistance;
        }

        public void Reset()
        {
        }
    }
}