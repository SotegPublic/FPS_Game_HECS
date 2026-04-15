using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "QuickShotPassiveAbilitySystem")]
    public sealed class QuickShotPassiveAbilitySystem : BaseWeaponPassiveAbilitySystem
    {
        [Required] private NameComponent nameComponent;
        [Required] private GradeComponent gradeComponent;
        [Required] private BuffAbilityConfigComponent config;
        [Required] private BuffAbilityModifierHolderComponent modifierHolder;
        [Required] private AbilityOwnerComponent ownerComponent;

        private DefaultFloatModifier shootIntervalModifier;

        public override void Execute(Entity owner = null, Entity target = null, bool enable = true)
        {
            if (enable)
            {
                ActivateBuff();
            }
            else
            {
                DeactivateBuff();
            }
        }

        private void ActivateBuff()
        {
            var abilitiesHolderComponent = ownerComponent.AbilityOwner.GetComponent<AbilitiesHolderComponent>();
            var abilities = abilitiesHolderComponent.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities.Items[i].ContainsMask<ShootingAbilityTagComponent>())
                {
                    AddBuffToWeapon(abilities.Items[i]);
                }
            }

            abilities.Dispose();
        }

        private void DeactivateBuff()
        {
            var abilitiesHolderComponent = ownerComponent.AbilityOwner.GetComponent<AbilitiesHolderComponent>();
            var abilities = abilitiesHolderComponent.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities.Items[i].ContainsMask<ShootingAbilityTagComponent>())
                {
                    RemoveBuffFromWeapon(abilities.Items[i]);
                }
            }

            abilities.Dispose();
        }

        private void AddBuffToWeapon(Entity ability)
        {
            var countersHolder = ability.GetComponent<CountersHolderComponent>();
            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.Cooldown, out var counter))
            {
                ability.Command(new AddCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    IsUnique = false,
                    Modifier = modifierHolder.Modifier,
                    Owner = Owner.GUID
                });
            }

            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.ShotInterval, out var shotsCounter))
            {
                ability.Command(new AddCounterModifierCommand<float>
                {
                    Id = shotsCounter.Id,
                    IsUnique = false,
                    Modifier = shootIntervalModifier,
                    Owner = Owner.GUID
                });
            }
        }

        private void RemoveBuffFromWeapon(Entity ability)
        {
            var countersHolder = ability.GetComponent<CountersHolderComponent>();
            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.Cooldown, out var counter))
            {
                ability.Command(new RemoveCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    Modifier = modifierHolder.Modifier,
                    Owner = Owner.GUID
                });
            }

            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.ShotInterval, out var shotsCounter))
            {
                ability.Command(new RemoveCounterModifierCommand<float>
                {
                    Id = shotsCounter.Id,
                    Modifier = shootIntervalModifier,
                    Owner = Owner.GUID
                });
            }
        }

        public override void InitSystem()
        {
            var value = config.GetValueByGrade(gradeComponent.Grade);
            var modifierIDString = nameComponent.DefaultName + gradeComponent.Grade;

            var modifier = new DefaultFloatModifier
            {
                GetCalculationType = config.CalculationType,
                GetModifierType = config.ModifierType,
                GetValue = value,
                ModifierGuid = Guid.NewGuid(),
                ModifierCounterID = CounterIdentifierContainerMap.Cooldown
            };

            modifierHolder.Modifier = modifier;

            shootIntervalModifier = new DefaultFloatModifier
            {
                GetCalculationType = config.CalculationType,
                GetModifierType = config.ModifierType,
                GetValue = value,
                ModifierGuid = Guid.NewGuid(),
                ModifierCounterID = CounterIdentifierContainerMap.ShotInterval
            };
        }

        public override void CommandReact(AddBuffToWeaponCommand command)
        {
            AddBuffToWeapon(command.Weapon);
        }

        public override void CommandReact(RemoveBuffFromWeaponCommand command)
        {
            RemoveBuffFromWeapon(command.Weapon);
        }
    }
}