using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Player, Doc.Shield, "this system set base parameters for shield")]
    public sealed class InitShieldSystem : BaseSystem, IInitAfterView
    {
        [Required] private LayersComponent layers;
        [Required] private HealthComponent health;
        [Required] private ShieldObjectProviderComponent shieldObjectProvider;

        public void InitAfterView()
        {
            var shieldObject = shieldObjectProvider.Get.gameObject;
            shieldObject.layer = layers.HideLayer.LayerID;
        }

        public override void InitSystem()
        {
            var player =  Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var healthValue = player.GetComponent<CountersHolderComponent>().GetFloatValue(CounterIdentifierContainerMap.Health);
            health.Setup(healthValue);

            Owner.World.Command(new UpdateHealthOnUICommand
            {
                MaxHP = health.CalculatedMaxValue,
                CurrentHP = health.Value,
            });
        }

        public void Reset()
        {
        }
    }
}