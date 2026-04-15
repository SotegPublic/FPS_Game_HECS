using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Shield, Doc.Damage, Doc.UI, "this system register damage end send command to update UI")]
    public sealed class ShieldUIUpdateSystem : BaseSystem, IReactCommand<DiffCounterCommand<float>>
    {
        public void CommandReact(DiffCounterCommand<float> command)
        {
            switch (command.Id)
            {
                case CounterIdentifierContainerMap.Health:
                    UpdateHealthCommand(command);
                    break;
                case CounterIdentifierContainerMap.ShieldRechargeCharge:
                case CounterIdentifierContainerMap.ShieldRechargePower:
                case CounterIdentifierContainerMap.ShieldRegenerationRate:
                case CounterIdentifierContainerMap.ShieldRegenerationPower:
                    //todo - for future features
                    break;
                default:
                    break;
            }
        }

        private void UpdateHealthCommand(DiffCounterCommand<float> command)
        {
            Owner.World.Command(new UpdateHealthOnUICommand
            {
                MaxHP = command.MaxValue,
                CurrentHP = command.Value,
            });
        }

        public override void InitSystem()
        {
        }
    }
}