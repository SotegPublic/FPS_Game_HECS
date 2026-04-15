using System;
using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Debuff, "LightningArcDebuffSystem")]
    public sealed class LightningArcDebuffSystem : BaseSystem, ILateUpdatable
    {
        [Required] private AssetRefIDHolderComponent assetRefHolder;
        [Required] private LightningArcFXConfigComponent fxConfig;

        private EntitiesFilter debuffFilter;
        private EntitiesFilter enemiesFilter;
        private HECSList<LightningArcDebuffFXContext> fXContexts = new HECSList<LightningArcDebuffFXContext>(32);

        public override void InitSystem()
        {
            debuffFilter = Owner.World.GetFilter(Filter.Get<LightningArcTagComponent>());
            enemiesFilter = Owner.World.GetFilter(Filter.Get<EnemyTagComponent>(), Filter.Get<IsDeadTagComponent>());
        }

        public void UpdateLateLocal()
        {
            foreach (var target in debuffFilter)
            {
                ShockEmAll(target);
            }

            CheckActiveFXes();
        }

        private void CheckActiveFXes()
        {
            var delta = Time.deltaTime;

            for(int i = fXContexts.Count - 1; i >= 0; i--)
            {
                ref var context = ref fXContexts[i];

                context.Timer -= delta;

                if(context.Timer <= 0)
                {
                    Owner.World.Command(new RemoveLineRenderFXGlobalCommand
                    {
                        EffectGuid = context.EffectGuid
                    });

                    fXContexts.RemoveSwap(context);
                }
            }
        }

        private void ShockEmAll(Entity target)
        {
            if (!target.IsAliveAndNotDead())
                return;

            var tag = target.GetComponent<LightningArcTagComponent>();
            var sqrRadius = tag.Radius * tag.Radius;

            enemiesFilter.ForceUpdateFilter();

            foreach(var enemy in enemiesFilter)
            {
                if (enemy.GUID == target.GUID)
                    continue;
                try
                {
                    if ((enemy.GetPosition() - target.GetPosition()).sqrMagnitude < sqrRadius)
                    {
                        var fx = assetRefHolder.GetRef(AssetRefIDMap.LightningArc);

                        var fxFrom = target.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.LightningArcTargetFXIdentifier);
                        var fxTo = enemy.GetComponent<VFXElementsHolderComponent>().GetFirstOrDefault(FXIdentifierMap.LightningArcTargetFXIdentifier);

                        var lightningArcCommand = new AddLineRenderFXGlobalCommand
                        {
                            AssetReference = fx,
                            EffectGuid = Guid.NewGuid(),
                            From = fxFrom,
                            To = fxTo,
                        };

                        fXContexts.Add(new LightningArcDebuffFXContext
                        {
                            EffectGuid = lightningArcCommand.EffectGuid,
                            Timer = fxConfig.ShowTime
                        });

                        Owner.World.Command(lightningArcCommand);

                        enemy.Command(new DamageCommand<float>(tag.Damage, tag.Damage, new DamageData
                        {
                            DamageDealer = tag.DamageDealer,
                            DamageKeeper = enemy
                        }));
                    }
                }
                catch (Exception ex)
                {
                    var t = 0;
                }
                
            }

            target.RemoveComponent<LightningArcTagComponent>();
        }
    }

    [Obsolete]
    public struct LightningArcDebuffFXContext : IEquatable<LightningArcDebuffFXContext>
    {
        public Guid EffectGuid;
        public float Timer;

        public bool Equals(LightningArcDebuffFXContext other)
        {
            return this.EffectGuid == other.EffectGuid;
        }

        public override bool Equals(object obj)
        {
            return obj is LightningArcDebuffFXContext context && Equals(context);
        }

        public override int GetHashCode()
        {
            return EffectGuid.GetHashCode();
        }
    }
}