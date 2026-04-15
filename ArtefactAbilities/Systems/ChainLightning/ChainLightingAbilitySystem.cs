using System;
using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Systems
{
    [RequiredAtContainer(typeof(IconComponent), typeof(AbilityTagComponent), typeof(AdditionalAbilityIndexComponent), typeof(AdditionalAbilityIndexComponent))]
    [Serializable]
    [Documentation(Doc.Abilities, "this ability spread chain lighting to some targets around")]
    public sealed class ChainLightingAbilitySystem : BaseAbilitySystem
    {
        [Required]
        public ChainLightningSettingsComponent ChainLightningSettingsComponent;

        [Required]
        public DamageComponent DamageComponent;

        [Required]
        public AssetRefIDHolderComponent AssetRefIDHolderComponent;

        [Single]
        public PoolingSystem PoolingSystem;

        [Required]
        public ActionsHolderComponent ActionsHolderComponent;

        [Required]
        public IDToViewHolderComponent IDToViewHolderComponent;

        private EntitiesFilter enemies;

        public override async void Execute(Entity owner = null, Entity target = null, bool enable = true)
        {
            if (!Owner.World.TryGetSingleComponent(out RawTargetComponent rawTarget))
                return;

            if (!Owner.World.TryGetSingleComponent(out MainCharacterTagComponent mainCharacter))
                return;

            ActionsHolderComponent.ExecuteAction(ActionIdentifierMap.StartAbility);

            var entity = Owner.GetAliveEntity();

            var getAssetref = AssetRefIDHolderComponent.GetRef(AssetRefIDMap.Artefact);

            var position = AbilityOwner.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.ArtefactPlace);

            var artefact = await PoolingSystem.GetViewFromPool(getAssetref, position.GetPosition, Quaternion.identity, position.transform);

            if (!entity.IsAlive)
            {
                ReturnView(artefact);
                return;
            }

            artefact.GetComponent<Animator>().SetTrigger(AnimParametersMap.Run);

            await UniTask.Delay(600);

            if (!entity.IsAlive)
            {
                ReturnView(artefact);
                return;
            }

            IDToViewHolderComponent.IDToViews.Add(new IDToView(AssetRefIDMap.Artefact, artefact));
            RunChainLightningScenario().Forget();
        }

        private async UniTask RunChainLightningScenario()
        {
            var position = AbilityOwner.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.ArtefactPlace);
            var fx = AssetRefIDHolderComponent.GetRef(AssetRefIDMap.LightningArc);
            enemies.ForceUpdateFilter();

            var aliveEntity = Owner.GetAliveEntity();

            if (enemies.Count == 0)
            {
                foreach (var view in this.IDToViewHolderComponent.IDToViews)
                    ReturnView(view.View);

                IDToViewHolderComponent.IDToViews.ClearFast();

                Owner.World.Command(new RemoveComponentByIDGlobalCommand
                {
                    AliveEntity = Owner.GetAliveEntity(),
                    ComponentIndex = ComponentProvider<OnCooldownTagComponent>.TypeIndex
                });

                return;
            }

            AliveEntity target = Owner.World.GetSingleComponent<RawTargetComponent>().Target;
            Entity closestEnemy = null;

            if (!target.IsAliveAndNotDead())
                closestEnemy = EntitiesHelper.GetClosestEntity(position.GetPosition, enemies);
            else
                closestEnemy = target;

            var closestAlive = closestEnemy.GetAliveEntity();

            var lightningToClosestGuid = Guid.NewGuid();
            IHavePosition targetPos = GetPosition(closestEnemy);


            var posClosestAlive = targetPos.GetPosition; 

            Owner.World.Command(new AddLineRenderFXGlobalCommand
            {
                AssetReference = fx,
                EffectGuid = lightningToClosestGuid,
                From = position,
                To = targetPos,
            });

            closestEnemy.Command(new DamageCommand<float>(DamageComponent.Value, DamageComponent.Value,
                new DamageData
                {
                    DamageDealer = AbilityOwner,
                    DamageKeeper = closestEnemy,
                }));

            Owner.World.Command(new SpawnFXToCoordCommand { Coord = targetPos.GetPosition, 
                FXId = FXIdentifierMap.LightningArcTargetFXIdentifier });
            Owner.World.Command(new CameraShakeCommand());

            await UniTask.Delay(ChainLightningSettingsComponent.ShowTime.ToMilliseconds());

            Owner.World.Command(new RemoveLineRenderFXGlobalCommand { EffectGuid = lightningToClosestGuid });

            await UniTask.Delay(400);

            foreach (var v in IDToViewHolderComponent.IDToViews)
            {
                ReturnView(v.View);
            }

            if (!aliveEntity.IsAliveAndNotDead())
                return;

            ChainLightning(aliveEntity, enemies, closestEnemy, posClosestAlive, fx);
        }

        private void ReturnView(GameObject view)
        {
            var animator = view.GetComponent<Animator>();

            animator.StopPlayback();
            animator.Rebind();

            view.transform.localScale = Vector3.one;
            view.transform.rotation = Quaternion.identity;
            view.transform.position = Vector3.zero;
            PoolingSystem.ReleaseView(view);
        }

        private async void ChainLightning(AliveEntity aliveEntity, EntitiesFilter enemies, AliveEntity closest, Vector3 closestPos, AssetReference fx)
        {
            var temp = HECSPooledArray<int>.GetArray(enemies.Count).GetSnapShot();

            if (closest.IsAlive)
                temp.Add(closest.Entity.Index);

            for (int i = 0; i < ChainLightningSettingsComponent.Bounces && i < enemies.Count; i++)
            {
                if (!aliveEntity.IsAliveAndNotDead())
                    return;

                if (closest.IsAliveAndNotDead() && closest.Entity.ContainsMask<VFXElementsHolderComponent>())
                    closestPos = closest.Entity.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.LightningArcTargetFXIdentifier).GetPosition;

                var closestEnemy = EntitiesHelper.GetClosestEntity(closestPos, enemies, CheckEntity);
                if (closestEnemy == null)
                    return;

                var lightningToClosestGuid = Guid.NewGuid();

                var posNextEnemy = GetPosition(closestEnemy);
                var fixCurrentPosition = Entity.Get("FXPosition").Init();

                var fixPos = fixCurrentPosition.GetOrAddComponent<TransformComponent>();
                fixPos.SetPosition(closestPos);

                IHavePosition targetPos = GetPosition(closestEnemy);

                temp.Add(closestEnemy.Index);

                Owner.World.Command(new AddLineRenderFXGlobalCommand
                {
                    AssetReference = fx,
                    EffectGuid = lightningToClosestGuid,
                    From = fixPos,
                    To = targetPos,
                });

                closestEnemy.Command(new DamageCommand<float>(DamageComponent.Value, DamageComponent.Value,
                    new DamageData
                    {
                        DamageDealer = AbilityOwner,
                        DamageKeeper = closestEnemy,
                    }));

                Owner.World.Command(new SpawnFXToCoordCommand
                {
                    Coord = targetPos.GetPosition,
                    FXId = FXIdentifierMap.LightningArcTargetFXIdentifier
                });

                Owner.World.Command(new CameraShakeCommand());

                closestPos = targetPos.GetPosition;
                closest = closestEnemy.GetAliveEntity();

                await UniTask.Delay(ChainLightningSettingsComponent.ShowTime.ToMilliseconds());

                Owner.World.Command(new RemoveLineRenderFXGlobalCommand { EffectGuid = lightningToClosestGuid });
                fixCurrentPosition.HecsDestroy();
            }

            bool CheckEntity(Entity entity)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp.Items[i] == entity.Index)
                        return false;

                    if (entity.ContainsMask<IsDeadTagComponent>())
                        return false;
                }

                return true;
            }

            temp.Release();
        }

        private IHavePosition GetPosition(Entity entity)
        {
            if (entity.TryGetComponent(out VFXElementsHolderComponent component))
            {
                var check = component.GetFirstOrDefault(FXIdentifierMap.LightningArcTargetFXIdentifier);
                if (check != null)
                    return check;
            }
            return entity.GetComponent<UnityTransformComponent>();
        }

        public override void InitSystem()
        {
            enemies = Owner.World.GetFilter(Filter.Get<EnemyTagComponent>(), Filter.Get<IsDeadTagComponent>());
            PoolingSystem.Warmup(AssetRefIDHolderComponent.GetRef(AssetRefIDMap.Artefact), 1).Forget();
            PoolingSystem.Warmup(AssetRefIDHolderComponent.GetRef(AssetRefIDMap.LightningArc), 3).Forget();
        }
    }
}