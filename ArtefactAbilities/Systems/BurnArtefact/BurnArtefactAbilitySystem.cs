using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, Doc.Abilities, "burn passive ability system")]
    public sealed class BurnArtefactAbilitySystem : BaseAbilitySystem
    {
        [Required] private ArtefactActivationTimeComponent activationTimeComponent;
        [Required] private BurnArtefactAbilityConfigComponent configComponent;
        [Required] private AbilityOwnerComponent abilityOwnerComponent;
        [Required] private IDToViewHolderComponent viewsHolder;
        [Required] private DamageComponent damageComponent;
        [Required] private AssetRefIDHolderComponent assetRefIDHolderComponent;
        [Required] private ActionsHolderComponent actionsHolderComponent;

        [Single] private PoolingSystem poolingSystem;

        private EntitiesFilter enemies;
        private HECSList<Guid> guids = new HECSList<Guid>(8);

        public override void Execute(Entity owner = null, Entity target = null, bool enable = true)
        {
            ActivateArtefactAsync().Forget();
        }

        private async UniTask ActivateArtefactAsync()
        {
            var isActivateSuccess = await ActivateArtefactVisual();
            if (!isActivateSuccess)
                return;

            actionsHolderComponent.ExecuteAction(ActionIdentifierMap.StartAbility);
            ExecuteArtefactAbility().Forget();
        }

        private async UniTask<bool> ActivateArtefactVisual()
        {
            var abilityOwner = abilityOwnerComponent.AbilityOwner;
            var position = abilityOwner.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.ArtefactPlace);
            var getAssetref = assetRefIDHolderComponent.GetRef(AssetRefIDMap.Artefact);
            var artefact = await poolingSystem.GetViewFromPool(getAssetref, position.GetPosition, Quaternion.identity, position.transform);

            var aliveEntity = Owner.GetAliveEntity();

            if (!aliveEntity.IsAlive)
            {
                ReturnView(artefact);
                return false;
            }

            artefact.GetComponent<Animator>().SetTrigger(AnimParametersMap.Run);

            await UniTask.Delay(activationTimeComponent.ActivationTime.ToMilliseconds());

            if (!aliveEntity.IsAlive)
            {
                ReturnView(artefact);
                return false;
            }

            viewsHolder.IDToViews.Add(new IDToView(AssetRefIDMap.Artefact, artefact));
            return true;
        }

        private async UniTask ExecuteArtefactAbility()
        {
            ActivateVFX();

            Owner.World.Command(new CameraShakeCommand());
            await UniTask.Delay(configComponent.ShowVFXTime.ToMilliseconds());

            for (int i = 0; i < guids.Count; i++)
            {
                Owner.World.Command(new RemoveLineRenderFXGlobalCommand { EffectGuid = guids[i] });
            }
            guids.ClearFast();

            foreach (var view in viewsHolder.IDToViews)
            {
                ReturnView(view.View);
            }
            viewsHolder.IDToViews.ClearFast();
        }

        private void ReturnView(GameObject view)
        {
            var animator = view.GetComponent<Animator>();

            animator.StopPlayback();
            animator.Rebind();

            view.transform.localScale = Vector3.one;
            view.transform.rotation = Quaternion.identity;
            view.transform.position = Vector3.zero;
            poolingSystem.ReleaseView(view);
        }

        private void ActivateVFX()
        {
            var fx = assetRefIDHolderComponent.GetRef(AssetRefIDMap.LightningArc);

            var abilityOwner = abilityOwnerComponent.AbilityOwner;
            var position = abilityOwner.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.ArtefactPlace);
            var startPosition = abilityOwner.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.ArtefactPlace);

            Owner.World.Command(new SpawnFXToCoordCommand
            {
                Coord = position.GetPosition,
                FXId = FXIdentifierMap.Fire,
            });

            DamageEnemies(position.GetPosition, abilityOwner);
        }

        private void DamageEnemies(Vector3 startPosition, Entity abilityOwner)
        {
            enemies.ForceUpdateFilter();
            foreach (var enemy in enemies)
            {
                if ((startPosition - enemy.GetPosition()).magnitude > configComponent.ExplosionRadius)
                    continue;

                enemy.Command(new DamageCommand<float>(damageComponent.Value, damageComponent.Value,
                    new DamageData
                    {
                        DamageDealer = abilityOwner,
                        DamageKeeper = enemy,
                    })
                );

                if (TryProcBuff(configComponent.ProcChance))
                {
                    var damage = damageComponent.Value * configComponent.PercentDamageValue;
                    var tag = enemy.GetOrAddComponent<BurnTagComponent>();
                    var timeBetweenTick = 1 / configComponent.FrequencyPerSecond;


                    tag.BurnTagContexts.Add(new BurnTagContext
                    {
                        Guid = Guid.NewGuid(),
                        DamageDealer = abilityOwner,
                        Damage = damage,
                        Duration = configComponent.Duration,
                        TimeBetweenTick = timeBetweenTick,
                        CurrentTimeBetweenTick = timeBetweenTick
                    });
                }
            }
        }

        private bool TryProcBuff(float procChance)
        {
            var rnd = UnityEngine.Random.Range(0f, 1f);

            return rnd <= procChance;
        }

        public override void InitSystem()
        {
            enemies = Owner.World.GetFilter(Filter.Get<EnemyTagComponent>(), Filter.Get<IsDeadTagComponent>());
            poolingSystem.Warmup(assetRefIDHolderComponent.GetRef(AssetRefIDMap.Artefact), 1).Forget();
        }

        public override void Dispose()
        {
            guids.Clear();
        }
    }
}