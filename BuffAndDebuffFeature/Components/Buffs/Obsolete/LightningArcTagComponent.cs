using System;
using HECSFramework.Core;

namespace Components
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Buff, Doc.Abilities, Doc.Tag, "lightning arc tag")]
    public sealed class LightningArcTagComponent : BaseComponent
    {
        public float Damage;
        public Entity DamageDealer;
        public float Radius;

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            Damage = 0f;
            Radius = 0f;
            DamageDealer = null;
        }
    }
}