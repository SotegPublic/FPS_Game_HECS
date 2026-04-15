using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEditor.Playables;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.PassiveAbilities, "AddShieldPassiveAbilitySystem")]
    public sealed class AddShieldPassiveAbilitySystem : BasePassiveAbilitySystem
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
            var countersHolder = shield.GetComponent<CountersHolderComponent>();
            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.Health, out var counter))
            {
                shield.Command(new AddCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    IsUnique = false,
                    Modifier = modifierHolder.Modifier,
                    Owner = Owner.GUID
                });
            }
        }

        private void DeactivateBuff()
        {
            var shield = Owner.World.GetEntityBySingleComponent<ShieldTagComponent>();
            var countersHolder = shield.GetComponent<CountersHolderComponent>();
            if (countersHolder.TryGetCounter<ICounterModifiable<float>>(CounterIdentifierContainerMap.Health, out var counter))
            {
                shield.Command(new RemoveCounterModifierCommand<float>
                {
                    Id = counter.Id,
                    Modifier = modifierHolder.Modifier,
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
                ModifierCounterID = CounterIdentifierContainerMap.Health
            };

            modifierHolder.Modifier = modifier;
        }
    }
}