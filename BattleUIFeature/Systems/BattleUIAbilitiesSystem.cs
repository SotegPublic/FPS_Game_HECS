using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using UnityEngine.UI;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, Doc.Buff, "this system control artefact abilities bar in battle UI")]
    public sealed class BattleUIAbilitiesSystem : BaseSystem, IReactGlobalCommand<UpdateAbilityUIAfterAddCommand>, IReactGlobalCommand<UpdateAbilityUIAfterRemoveCommand>,
        IReactGlobalCommand<UpdateEnergyUICommand>
    {
        [Required] private UIAccessProviderComponent uiAccessProvider;
        [Required] private BattleUIBuffSystemStateComponent stateComponent;
        [Required] private UIAccessPrfbProviderComponent prefabsProvider;


        public void CommandGlobalReact(UpdateAbilityUIAfterAddCommand command)
        {
            var buffEntityGuid = command.Ability.GUID;
            AddBuffToUI(buffEntityGuid, command.Ability);
        }

        private void AddBuffToUI(Guid abilityGuid, Entity abilityEntity)
        {
            var abilityIcon = abilityEntity.GetComponent<IconComponent>().Icon;
            CreateIcon(abilityGuid, abilityIcon, abilityEntity);
        }

        private void CreateIcon(Guid buffGuid, Sprite buffIcon, Entity itemEntity)
        {
            var abilitiesRoot = uiAccessProvider.Get.GetRectTransform(UIAccessIdentifierMap.BuffsRoot);
            var abilityPref = prefabsProvider.Get.GetPrefab(UIAccessIdentifierMap.BuffPref);
            var abilityIconMonoComponent = MonoBehaviour.Instantiate(abilityPref, abilitiesRoot).GetComponent<AbilityIconMonoComponent>();
            var abilityCost = itemEntity.GetComponent<EnergyCostComponent>().Cost;

            abilityIconMonoComponent.Init(buffIcon, abilityCost);

            abilityIconMonoComponent.AbilityButton.onClick.AddListener(() => React(itemEntity));
            stateComponent.Icons.Add(buffGuid, abilityIconMonoComponent);
        }

        private void React(Entity itemEntity)
        {
            itemEntity.Command(new ExecuteAbilityCommand { Enabled = true });
        }

        public void CommandGlobalReact(UpdateAbilityUIAfterRemoveCommand command)
        {
            var abilityEntityGuid = command.Ability.GUID;
            stateComponent.Icons[abilityEntityGuid].AbilityButton.onClick.RemoveAllListeners();

            GameObject.Destroy(stateComponent.Icons[abilityEntityGuid].gameObject);
            stateComponent.Icons.Remove(abilityEntityGuid);
        }

        public void CommandGlobalReact(UpdateEnergyUICommand command)
        {
            var abilityButton = stateComponent.Icons[command.AbilityGuid];
            abilityButton.SetEnergyProgress(command.CurrentEnergy);
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.UI, Doc.Artefact, "UpdateAbilityUIAfterAddCommand")]
    public struct UpdateAbilityUIAfterAddCommand : IGlobalCommand
    {
        public Entity Ability;
    }

    [Serializable]
    [Documentation(Doc.UI, Doc.Artefact, "UpdateAbilityUIAfterRemoveCommand")]
    public struct UpdateAbilityUIAfterRemoveCommand : IGlobalCommand
    {
        public Entity Ability;
    }
}