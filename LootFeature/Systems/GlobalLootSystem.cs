using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Loot, Doc.Global, "this system is responsible for processing loot spawn commands")]
    public sealed class GlobalLootSystem : BaseSystem, IReactGlobalCommand<GlobalSpawnLootCommand>
    {
        [Required] public LootVariantsHolderComponent LootVariantsHolder;

        public override void InitSystem()
        {
        }

        public void CommandGlobalReact(GlobalSpawnLootCommand command)
        {
            if (!command.From.Entity.TryGetComponent(out EnemyLootComponent lootConfigComponent))
                return;
            if(!LootVariantsHolder.TryGetContainerByID(lootConfigComponent.LootDropID, out var container))
                return;

            var position = command.From.Entity.GetPosition();
            
            SpawnLootAsync(position, container, lootConfigComponent.LootGrade).Forget();
        }

        private async UniTask SpawnLootAsync(Vector3 position, EntityContainer container, int lootGrade)
        {
            var gloabalLootEntity = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>();
            gloabalLootEntity.GetOrAddComponent<VisualLocalLockComponent>().AddLock();

            var lootActor = await container.GetActor(position: position);
            lootActor.Init();

            await UniTask.DelayFrame(1);
            
            lootActor.Entity.Command(new CalculateRewardsCommand { GradeID = lootGrade });

            await new WaitFor<LootCalculatedTagComponent>(lootActor.Entity).RunJob();

            lootActor.Entity.Command(new SpawnRewardsViewsCommand());

            gloabalLootEntity.GetOrAddComponent<VisualLocalLockComponent>().Remove();
        }
    }
}