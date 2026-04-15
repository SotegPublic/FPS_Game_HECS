using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Shield, Doc.Buff, "ShieldReflectSystem")]
    public sealed class ShieldReflectSystem : BaseSystem, IReactCommand<AfterCommand<DamageCommand<float>>>
    {
        [Required] private ShieldReflectPowerComponent reflectPowerComponent;

        public void CommandReact(AfterCommand<DamageCommand<float>> command)
        {
            if (!Owner.TryGetComponent<ShieldReflectTagComponent>(out var tag))
                return;

            if (!command.Value.DamageData.DamageDealer.TryGetComponent<AbilityOwnerComponent>(out var damageOwner))
                return;

            if (!damageOwner.AbilityOwner.ContainsMask<EnemyTagComponent>())
                return;

            var playerCharacter = Owner.World.GetEntityBySingleComponent<MainCharacterTagComponent>();
            var abilitiesHolder = playerCharacter.GetComponent<AbilitiesHolderComponent>();
            var reflectAbility = abilitiesHolder.IndexToAbility[AdditionalAbilityIdentifierMap.ShieldReflect];

            var reflectPercent = reflectPowerComponent.Value;
            var reflectedDamage = command.Value.DamageValue * (reflectPercent * 0.01f);

            var abilityDamage = reflectAbility.GetOrAddComponent<DamageComponent>();
            abilityDamage.SetValue(reflectedDamage);

            reflectAbility.Command(new ExecuteAbilityCommand { Enabled = true, Owner = playerCharacter, Target = damageOwner.AbilityOwner });
        }

        public override void InitSystem()
        {
        }
    }
}