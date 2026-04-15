using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [RequiredAtContainer(typeof(WeaponBuffTagComponent))]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "BaseWeaponPassiveAbilitySystem")]
    public abstract class BaseWeaponPassiveAbilitySystem : BasePassiveAbilitySystem, IReactCommand<AddBuffToWeaponCommand>, IReactCommand<RemoveBuffFromWeaponCommand>
    {
        public abstract void CommandReact(AddBuffToWeaponCommand command);

        public abstract void CommandReact(RemoveBuffFromWeaponCommand command);
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "AddBuffToWeaponCommand")]
    public struct AddBuffToWeaponCommand : ICommand
    {
        public Entity Weapon;
    }

    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "RemoveBuffFromWeaponCommand")]
    public struct RemoveBuffFromWeaponCommand : ICommand
    {
        public Entity Weapon;
    }
}
