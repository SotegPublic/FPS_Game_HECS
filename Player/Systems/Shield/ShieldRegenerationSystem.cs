using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Shield, "ShieldRegenerationSystem")]
    public sealed class ShieldRegenerationSystem : BaseSystem, IUpdatable
    {
        [Required] private ShieldRegenerationRateComponent regenerationRateComponent;
        [Required] private HealthComponent healthComponent;
        [Required] private ShieldRegenerationPowerComponent regenerationComponent;

        private float currentInterval;

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if (regenerationComponent.Value <= 0)
                return;

            currentInterval += Time.deltaTime;
            if (currentInterval < regenerationRateComponent.Value)
                return;

            healthComponent.ChangeValue(regenerationComponent.Value);

            currentInterval = 0f;
        }
    }
}