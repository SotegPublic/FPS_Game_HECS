using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "ShieldReflectPassiveAbilitySystem")]
    public sealed class ShieldReflectPassiveAbilitySystem : BasePassiveAbilitySystem
    {
        [Required] private NameComponent nameComponent;
        [Required] private GradeComponent gradeComponent;
        [Required] private BuffAbilityConfigComponent config;
        [Required] private BuffAbilityModifierHolderComponent modifierHolder;

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
            var shield = Owner.World.GetEntityBySingleComponent<ShieldTagComponent>();
            var tag = shield.GetOrAddComponent<ShieldReflectTagComponent>();
            var shieldCountersHolder = shield.GetComponent<CountersHolderComponent>();

            if (shieldCountersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.ShieldReflectPower, out var counter))
            {
                shield.Command(new AddCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    IsUnique = false,
                    Modifier = modifierHolder.Modifier,
                    Owner = Owner.GUID
                });

                tag.AddStack();
            }
        }

        private void DeactivateBuff()
        {
            var shield = Owner.World.GetEntityBySingleComponent<ShieldTagComponent>();
            var tag = shield.GetOrAddComponent<ShieldReflectTagComponent>();
            var shieldCountersHolder = shield.GetComponent<CountersHolderComponent>();

            if (shieldCountersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.ShieldReflectPower, out var counter))
            {
                shield.Command(new RemoveCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    Modifier = modifierHolder.Modifier,
                    Owner = Owner.GUID
                });
                tag.RemoveStack();
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
                ModifierCounterID = CounterIdentifierContainerMap.ShieldReflectPower
            };

            modifierHolder.Modifier = modifier;
        }
    }
}