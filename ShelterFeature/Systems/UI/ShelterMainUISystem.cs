using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.UI, "shelter main ui system")]
    public sealed class ShelterMainUISystem : BaseSystem, IAfterEntityInit, IReactGlobalCommand<AfterCommand<ToggleClickCommand>>,
        IReactGlobalCommand<UpdateShelterUICounterCommand>, IReactGlobalCommand<UpdateVisualRewardCounterCommand>
    {
        [Required] private UIAccessProviderComponent accessProvider;

        private Entity shelterFeature;

        private const ToggleGroupID TOGGLE_GROUP_ID = ToggleGroupID.One;

        public override void InitSystem()
        {
        }

        public void AfterEntityInit()
        {
            shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();

            Owner.Command(new ToggleClickCommand { ToggleGroupID = TOGGLE_GROUP_ID, ToggleIndex = UIAccessIdentifierMap.ShelterUI });

            UpdateCountersText();
        }

        private void UpdateCountersText()
        {
            var shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();
            var energyflowCounter = shelterFeature.GetComponent<ShelterEnergyflowCounterComponent>();
            var cashflowCounter = shelterFeature.GetComponent<ShelterCashflowCounterComponent>();

            UpdateUICounter(UIAccessIdentifierMap.Energyflow, energyflowCounter.Value, energyflowCounter.CalculatedMaxValue);
            UpdateUICounter(UIAccessIdentifierMap.Cashflow, cashflowCounter.Value, cashflowCounter.CalculatedMaxValue);

            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();

            var survivorsCounter = player.GetComponent<SurvivorsCounterComponent>();
            var energyCounter = player.GetComponent<ShelterEnergyCounterComponent>();
            var yellowCrystalsCounter = player.GetComponent<YellowCrystalsCounterComponent>();
            var greenCrystalsCounter = player.GetComponent<GreenCrystalsCounterComponent>();
            var moneyCounter = player.GetComponent<MoneyCounterComponent>();

            UpdateUICounter(UIAccessIdentifierMap.Survivors, survivorsCounter.Value, survivorsCounter.CalculatedMaxValue);
            UpdateUICounter(UIAccessIdentifierMap.ShelterEnergy, energyCounter.Value, energyCounter.CalculatedMaxValue);
            UpdateUICounter(UIAccessIdentifierMap.YellowCrystals, yellowCrystalsCounter.Value, 0);
            UpdateUICounter(UIAccessIdentifierMap.GreenCrystals, greenCrystalsCounter.Value, 0);
            UpdateUICounter(UIAccessIdentifierMap.Money, moneyCounter.Value, 0);
        }

        public void CommandGlobalReact(UpdateShelterUICounterCommand command)
        {
            UpdateUICounter(command.CounterID, command.Value, command.MaxValue);
        }

        private void UpdateUICounter(int counterID, int value, int maxValue)
        {
            var targetText = accessProvider.Get.GetTextMeshProUGUI(counterID);
            var text = "";

            switch (counterID)
            {
                case UIAccessIdentifierMap.Survivors:
                case UIAccessIdentifierMap.ShelterEnergy:
                    text = value + "/" + maxValue;
                    targetText.text = text;
                    break;
                case UIAccessIdentifierMap.Cashflow:
                case UIAccessIdentifierMap.Energyflow:
                    text = value + " per min";
                    targetText.text = text;
                    break;
                default:
                    targetText.text = value.ToString();
                    break;
            }
        }

        public void CommandGlobalReact(UpdateVisualRewardCounterCommand command)
        {
            var targetText = accessProvider.Get.GetTextMeshProUGUI(command.CounterID);

            if (targetText != null)
            {
                targetText.text = command.Amount.ToString();
            }
        }

        public void CommandGlobalReact(AfterCommand<ToggleClickCommand> command)
        {
            if(command.Value.ToggleGroupID == TOGGLE_GROUP_ID)
                ShowPanel(command.Value.ToggleIndex);
        }

        private void ShowPanel(int panelID)
        {
            var targetCanvasGroup = accessProvider.Get.GetCanvasGroup(panelID);

            foreach(var cg in accessProvider.Get.CanvasGroups)
            {
                if (cg.UIAccessIdentifier != UIAccessIdentifierMap.Disable)
                    continue;

                if (cg.Value == targetCanvasGroup)
                    continue;

                HideCanvasGroup(cg.Value);
            }

            Owner.World.Command(new ShowShelterPanelCommand { PanelID = panelID });
            ShowCanvasGroup(targetCanvasGroup);

            if(panelID == UIAccessIdentifierMap.ShelterUI)
            {
                shelterFeature.GetOrAddComponent<MainShelterUIActiveTagComponent>();
            }
            else
            {
                if (shelterFeature.TryGetComponent<MainShelterUIActiveTagComponent>(out var component))
                    shelterFeature.RemoveComponent(component);
            }

            static void ShowCanvasGroup(CanvasGroup group)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            static void HideCanvasGroup(CanvasGroup group)
            {
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();

            var shelterUIBtn = accessProvider.Get.GetButton(UIAccessIdentifierMap.ShelterUI);
            var selectEnterBtn = accessProvider.Get.GetButton(UIAccessIdentifierMap.Missions);

            shelterUIBtn.onClick.RemoveAllListeners();
            selectEnterBtn.onClick.RemoveAllListeners();
        }
    }
}