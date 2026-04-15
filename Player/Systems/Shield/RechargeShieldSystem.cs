using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Shield, "RechargeShieldSystem")]
    public sealed class RechargeShieldSystem : BaseSystem, IReactCommand<ShieldRechargeCommand>
    {
        [Required] private ShieldRechargePowerComponent rechargePowerComponent;
        [Required] private ShieldRechargeChargesComponent rechargeChargesComponent;
        [Required] private HealthComponent healthComponent;

        public void CommandReact(ShieldRechargeCommand command)
        {
            Owner.RemoveComponent<IsDeadTagComponent>();

            var rechargeValue = healthComponent.CalculatedMaxValue * (rechargePowerComponent.Value * 0.01f);
            healthComponent.ChangeValue(rechargeValue);
            rechargeChargesComponent.ChangeValue(-1);
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shield, "ShieldRechargeCommand")]
    public struct ShieldRechargeCommand : ICommand
    {
    }
}