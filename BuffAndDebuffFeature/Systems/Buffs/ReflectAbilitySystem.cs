using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Abilities, Doc.Damage, "ReflectAbilitySystem")]
    public sealed class ReflectAbilitySystem : BaseAbilitySystem 
    {
        [Required] private DamageComponent damageComponent;

        public override void Execute(Entity owner = null, Entity target = null, bool enable = true)
        {
            var damageData = new DamageData
            {
                DamageDealer = Owner,
                DamageKeeper = target
            };

            target.Command(new DamageCommand<float>(damageComponent.Value, damageComponent.Value, damageData));
        }

        public override void InitSystem()
        {
        }
    }
}