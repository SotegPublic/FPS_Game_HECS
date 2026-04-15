using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, Doc.Resources, "this system control resources panel")]
    public sealed class BattleUIResourcesSystem : BaseSystem, IReactCommand<UpdateUIIntCounterCommand>, IReactGlobalCommand<UpdateVisualRewardCounterCommand>
    {
        [Required] private UIAccessProviderComponent uiAccessProvider;
        [Required] private CountersHolderComponent countersHolder;
        [Required] private BattleUIResourcesSystemStateComponent stateComponent;
        [Required] private UIAccessPrfbProviderComponent prefabsProvider;

        public void CommandReact(UpdateUIIntCounterCommand command)
        {
            AddOrUpdateCounter(command.CounterID, command.Amount);
        }

        private void AddOrUpdateCounter(int counterID, int amount)
        {
            if (!stateComponent.Icons.ContainsKey(counterID))
            {
                CreateCounterAndIcon(counterID);
            }

            if (!countersHolder.TryGetIntCounter(counterID, out var counter))
                return;

            counter.ChangeValue(amount);
            stateComponent.Icons[counterID].ChangeCount(counter.Value);
        }

        private void CreateCounterAndIcon(int counterID)
        {
            var iconsConfig = Owner.World.GetEntityBySingleComponent<GlobalRewarsFeatureTagComponent>().GetComponent<ResourcesIconConfigsComponent>();

            var resourceID = counterID;
            var resourceIcon = iconsConfig.GetIcon(resourceID);

            var resourcesRoot = uiAccessProvider.Get.GetRectTransform(UIAccessIdentifierMap.ResourcesRoot);
            var resourcePref = prefabsProvider.Get.GetPrefab(UIAccessIdentifierMap.ResourcePref);
            var resourceIconMonoComponent = MonoBehaviour.Instantiate(resourcePref, resourcesRoot).GetComponent<ResourceIconMonoComponent>();

            resourceIconMonoComponent.Init(resourceIcon);

            stateComponent.Icons.Add(resourceID, resourceIconMonoComponent);
            countersHolder.AddCounter(new DefaultIntCounter(counterID));
        }

        public void CommandGlobalReact(UpdateVisualRewardCounterCommand command)
        {
            var resourcesHolder = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            if (command.To != resourcesHolder)
                return;

            AddOrUpdateCounter(command.CounterID, command.Amount);
        }

        public override void InitSystem()
        {
        }
    }
}