using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

public static partial class DocFeature
{
    public const string BuffFeature = "BuffFeature";
}


namespace Systems
{
	[Serializable][Documentation(DocFeature.BuffFeature, "this system is main system of buffs, and shoud be on the entity like character or other entity with abilities")]
    public sealed class EnableBuffSystem : BaseSystem, IReactCommand<AddBuffByEntity>, IReactCommand<AddBuffByEntityContainer>,
        IReactCommand<RemoveBuffByEntity>, IReactCommand<RemoveBuffByEntityContainer>, IReactCommand<DisableBuffByEntity>,
        IReactCommand<EnableBuffByEntity>, IReactCommand<RemoveAllBuffs>
    {
        [Required]
        public AbilitiesHolderComponent AbilitiesHolderComponent;

        public void CommandReact(AddBuffByEntity command)
        {
            AbilitiesHolderComponent.AddPassiveAbility(command.Buff, command.From, command.To);

            Owner.World.Command(new AfterCommand<AddBuffByEntity> { Value = command });
        }

        public void CommandReact(AddBuffByEntityContainer command)
        {
            var ability = command.Buff.GetEntity();
            AbilitiesHolderComponent.AddPassiveAbility(ability, command.From, command.To);

            Owner.World.Command(new AfterCommand<AddBuffByEntity> { Value = new AddBuffByEntity
            {
                Buff = ability,
                From = command.From,
                To = command.To
            }});
        }

        public void CommandReact(RemoveBuffByEntity command)
        {
            command.Buff.Command(new ExecutePassiveAbilityCommand { Enabled = false, Owner = null, Target = null });

            Owner.World.Command(new AfterCommand<RemoveBuffByEntity> { Value = command });

            AbilitiesHolderComponent.RemoveAbility(command.Buff);
        }


        public void CommandReact(RemoveBuffByEntityContainer command)
        {
            var abilities = AbilitiesHolderComponent.GetAllAbilities();
            var buffID = command.Buff.GetComponent<BuffAbilityTagComponent>().BuffID;
            
            foreach(var ability in abilities.Items)
            {
                if (!ability.TryGetComponent<BuffAbilityTagComponent>(out var abilityTag))
                    continue;
                if (abilityTag.BuffID != buffID)
                    continue;

                ability.Command(new ExecutePassiveAbilityCommand { Enabled = false, Owner = null, Target = null });
                AbilitiesHolderComponent.RemoveAbility(ability);

                Owner.World.Command(new AfterCommand<RemoveBuffByEntity> { Value = new RemoveBuffByEntity 
                { 
                    Buff = ability
                }});
                break;
            }
        }

        public void CommandReact(EnableBuffByEntity command)
        {
            command.Buff.Command(new ExecutePassiveAbilityCommand { Enabled = true, Owner = Owner, Target = Owner });
        }

        public void CommandReact(DisableBuffByEntity command)
        {
            command.Buff.Command(new ExecutePassiveAbilityCommand { Enabled = false, Owner = null, Target = null });
        }

        public void CommandReact(RemoveAllBuffs command)
        {
            var abilities = AbilitiesHolderComponent.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (!abilities.Items[i].ContainsMask<BuffAbilityTagComponent>())
                    continue;

                abilities.Items[i].Command(new ExecutePassiveAbilityCommand { Enabled = false, Owner = null, Target = null });
                AbilitiesHolderComponent.RemoveAbility(abilities.Items[i]);
            }

            Owner.World.Command(new AfterCommand<RemoveAllBuffs> { Value = command });
            abilities.Dispose();
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    public struct AddBuffByEntity : ICommand
    {
        public Entity From;
        public Entity To;
        public Entity Buff;
    }

    public struct AddBuffByEntityContainer : ICommand
    {
        public Entity From;
        public Entity To;
        public EntityContainer Buff;
    }

    public struct EnableBuffByEntity : ICommand
    {
        public Entity Buff;
    }

    public struct DisableBuffByEntity : ICommand
    {
        public Entity Buff;
    }

    public struct RemoveBuffByEntity : ICommand
    {
        public Entity Buff;
    }

    public struct RemoveBuffByEntityContainer : ICommand
    {
        public EntityContainer Buff;
    }

    public struct RemoveAllBuffs : ICommand
    {
    }
}