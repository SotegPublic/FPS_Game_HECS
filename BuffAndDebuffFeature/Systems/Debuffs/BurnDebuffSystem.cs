using System;
using System.Collections.Generic;
using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.Debuff, "BurnDebuffSystem")]
    public sealed class BurnDebuffSystem : BaseSystem, ILateUpdatable
    {
        [Required] private BurnConfigComponent configComponent;

        private EntitiesFilter filter;

        public override void InitSystem()
        {
            filter = Owner.World.GetFilter(Filter.Get<BurnTagComponent>());
        }

        public void UpdateLateLocal()
        {
            foreach (var target in filter)
            {
                if (!target.IsAliveAndNotDead())
                    continue;

                var tag = target.GetComponent<BurnTagComponent>();

                RemoveEndedStacks(tag);

                if (tag.BurnTagContexts.Count == 0)
                {
                    target.RemoveComponent<BurnTagComponent>();
                    continue;
                }

                CheckStacksCount(tag);
                BurnEmAll(target, tag);
            }
        }

        private void RemoveEndedStacks(BurnTagComponent tag)
        {
            for (int i = tag.BurnTagContexts.Count - 1; i >= 0; i--)
            {
                ref var currentContext = ref tag.BurnTagContexts[i];

                if (currentContext.Duration <= 0)
                {
                    tag.BurnTagContexts.RemoveAtSwap(i);
                }
            }

            if(tag.BurnTagContexts.Count <= 0)
            {
                tag.RemoveBurnTag();
            }
        }

        private void CheckStacksCount(BurnTagComponent tag)
        {
            if (tag.BurnTagContexts.Count <= configComponent.MaxStacksCount)
                return;

            for(int i = 0; i < tag.BurnTagContexts.Count - configComponent.MaxStacksCount; i++)
            {
                tag.BurnTagContexts.RemoveAtSwap(0);
            }
        }

        private void BurnEmAll(Entity target, BurnTagComponent tag)
        {
            for (int i = 0; i < tag.BurnTagContexts.Count; i++)
            {
                ref var currentContext = ref tag.BurnTagContexts[i];

                currentContext.Duration -= Time.deltaTime;
                currentContext.CurrentTimeBetweenTick -= Time.deltaTime;

                if (currentContext.CurrentTimeBetweenTick > 0)
                    continue;

                currentContext.CurrentTimeBetweenTick = currentContext.TimeBetweenTick + currentContext.CurrentTimeBetweenTick;

                target.Command(new DamageCommand<float>(currentContext.Damage, currentContext.Damage, new DamageData
                {
                    DamageDealer = currentContext.DamageDealer,
                    DamageKeeper = target,
                }));
            }
        }
    }
}