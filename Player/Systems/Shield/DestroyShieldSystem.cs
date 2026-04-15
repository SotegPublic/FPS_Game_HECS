using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Shield, Doc.Player, "this system transmite death command to player character when shield was destroy")]
    public sealed class DestroyShieldSystem : BaseSystem, IReactCommand<IsDeadCommand> 
    {
        [Required] private ShieldRechargeChargesComponent rechargeChargesComponent;

        public void CommandReact(IsDeadCommand command)
        {
            if (rechargeChargesComponent.Value < 1)
            {
                Owner.World.GetEntityBySingleComponent<PlayerCharacterComponent>().Command(command);
            }
            else
            {
                Owner.Command(new ShieldRechargeCommand());
            }
        }

        public override void InitSystem()
        {
        }
    }
}