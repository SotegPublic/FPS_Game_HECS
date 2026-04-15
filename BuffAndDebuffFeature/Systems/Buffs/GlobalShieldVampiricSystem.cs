using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Buff, "this system check damage commands and regen shield if is there required tag")]
    public sealed class GlobalShieldVampiricSystem : BaseSystem, IPriorityLateUpdatable
    {
        [Required] private GlobalDamageCommandsHolderComponent damageCommandsHolderComponent;

        public int Priority => 1;

        public override void InitSystem()
        {
        }

        public void PriorityLateUpdateLocal()
        {
            if (damageCommandsHolderComponent.DamageCommands.Count == 0)
                return;
            
            var shieldHealth = Owner.World.GetEntityBySingleComponent<ShieldTagComponent>().GetComponent<HealthComponent>();

            foreach(var command in damageCommandsHolderComponent.DamageCommands)
            {
                if(command.DamageData.DamageDealer.TryGetComponent<ShieldVampiricTagComponent>(out var tag))
                {
                    var countersHolder = command.DamageData.DamageDealer.GetComponent<CountersHolderComponent>();
                    var vampiricPercent = countersHolder.GetFloatValue(CounterIdentifierContainerMap.ShieldVampiricPower);
                    var damage = command.DamageValue;

                    var vampiricValue = damage * (vampiricPercent * 0.01f);
                    shieldHealth.ChangeValue(vampiricValue);
                }
            }
        }
    }
}