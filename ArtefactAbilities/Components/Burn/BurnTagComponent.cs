using System;
using System.Collections.Generic;
using HECSFramework.Core;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.Abilities, Doc.Tag, "burn tag")]
    public sealed class BurnTagComponent : BaseComponent
    {
        public HECSList<BurnTagContext> BurnTagContexts = new HECSList<BurnTagContext>(8);

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            BurnTagContexts.ClearFast();
        }

        public void RemoveBurnTag()
        {
            Owner.RemoveComponent(this);
        }
    }

    public struct BurnTagContext : IEquatable<BurnTagContext>
    {
        public Guid Guid;
        public Entity DamageDealer;
        public float Damage;
        public float Duration;
        public float TimeBetweenTick;
        public float CurrentTimeBetweenTick;

        public override bool Equals(object obj)
        {
            return obj is BurnTagContext context &&
                   Guid.Equals(context.Guid) &&
                   EqualityComparer<Entity>.Default.Equals(DamageDealer, context.DamageDealer);
        }

        public bool Equals(BurnTagContext context)
        {
            return Guid.Equals(context.Guid) &&
                   EqualityComparer<Entity>.Default.Equals(DamageDealer, context.DamageDealer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Guid, DamageDealer);
        }
    }
}