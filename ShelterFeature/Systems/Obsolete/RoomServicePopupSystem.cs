using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Recorder.OutputPath;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.UI, "this system show info about resources which needed for selected room")]
    public sealed class RoomServicePopupSystem : BaseSystem, IAfterEntityInit, IReactGlobalCommand<ShowRoomServicePopup>
    {
        [Required] private UIAccessProviderComponent uIAccess;
        [Required] private UIAccessPrfbProviderComponent prfbProvider;
        [Required] private RoomServicePopupSystemStateComponent stateComponent;

        private Entity shelterFeature;
        private UIAccessMonoComponent shelterUIAccess;

        public void AfterEntityInit()
        {
            shelterUIAccess = uIAccess.Get.GetUIAccess(UIAccessIdentifierMap.ShelterUI);
            shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();

            var canvasGroup = shelterUIAccess.GetCanvasGroup(UIAccessIdentifierMap.RoomService);
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            stateComponent.OnAddResourceButtonClick += AddResources;

            var closeBtn = shelterUIAccess.GetButton(UIAccessIdentifierMap.RoomResourcesExitButton);
            closeBtn.onClick.AddListener(HideRoomServicePopup);
        }

        private void HideRoomServicePopup()
        {
            var canvasGroup = shelterUIAccess.GetCanvasGroup(UIAccessIdentifierMap.RoomService);
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            
            stateComponent.ClearContexts();
            shelterFeature.RemoveComponent<BlockShelterSystemTagComponent>();
        }

        public void CommandGlobalReact(ShowRoomServicePopup command)
        {
            shelterFeature.GetOrAddComponent<BlockShelterSystemTagComponent>();

            var resourceRowPrfb = prfbProvider.Get.GetPrefab(UIAccessIdentifierMap.RoomService);
            var root = shelterUIAccess.GetRectTransform(UIAccessIdentifierMap.RoomService);

            var roomServiceConfig = command.Room.GetComponent<RoomServiceConfigComponent>();

            for(int i = 0; i < roomServiceConfig.ResourcesForService.Length; i++)
            {
                var rowConfig = roomServiceConfig.ResourcesForService[i];

                var row = MonoBehaviour.Instantiate(resourceRowPrfb, root); //todo - pool
                var rowUIAccess = row.GetComponent<UIAccessMonoComponent>();

                CreateRow(command, rowConfig, rowUIAccess);
                stateComponent.RegisterRow(command.Room, rowUIAccess, rowConfig);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(root);

            var canvasGroup = shelterUIAccess.GetCanvasGroup(UIAccessIdentifierMap.RoomService);
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

        private void AddResources(RowContext context)
        {
            var roomCountersHolder = context.Room.GetComponent<CountersHolderComponent>();
            var playerCountersHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<CountersHolderComponent>();
            var playerCounter = playerCountersHolder.GetOrAddIntCounter(context.RowConfig.ResourceID);
            var targetCounter = roomCountersHolder.GetOrAddIntCounter(context.RowConfig.ResourceID);
            
            var freeSpace = Mathf.Clamp(context.RowConfig.ResourceCap - targetCounter.Value, 0, context.RowConfig.ResourceCap);
            var addedValue = playerCounter.Value - freeSpace <= 0 ? playerCounter.Value : freeSpace;

            targetCounter.ChangeValue(addedValue);
            playerCounter.ChangeValue(-addedValue);

            UpdateProgressBar(context.Room, context.RowConfig, context.RowUIAccess);
        }

        private void CreateRow(ShowRoomServicePopup command, ResourceForServiceModel rowConfig, UIAccessMonoComponent rowUIAccess)
        {
            var resourceIcon = Owner.World.GetEntityBySingleComponent<GlobalRewarsFeatureTagComponent>().
                GetComponent<ResourcesIconConfigsComponent>().
                GetIcon(rowConfig.ResourceID);

            var image = rowUIAccess.GetImage(UIAccessIdentifierMap.Resources);
            var name = rowUIAccess.GetTextMeshProUGUI(UIAccessIdentifierMap.Resources);

            image.sprite = resourceIcon;
            name.text = IdentifierToStringMap.IntToString[rowConfig.ResourceID];
            UpdateProgressBar(command.Room, rowConfig, rowUIAccess);
        }

        private void UpdateProgressBar(Entity room, ResourceForServiceModel rowConfig, UIAccessMonoComponent rowUIAccess)
        {
            var progressBar = rowUIAccess.GetImage(UIAccessIdentifierMap.Progressbar);
            var currentCounterValue = room.GetComponent<CountersHolderComponent>().GetIntValue(rowConfig.ResourceID);
            var valueCap = rowConfig.ResourceCap;

            var currentFill = 1f * currentCounterValue / valueCap;
            progressBar.fillAmount = currentFill;
        }

        public override void InitSystem()
        {
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();

            stateComponent.OnAddResourceButtonClick -= AddResources;

            var closeBtn = shelterUIAccess.GetButton(UIAccessIdentifierMap.RoomResourcesExitButton);
            closeBtn.onClick.RemoveListener(HideRoomServicePopup);
        }
    }
}

namespace Commands
{
    [Obsolete]
    [Serializable][Documentation(Doc.UI, "this command show room service popup")]
    public struct ShowRoomServicePopup : IGlobalCommand
    {
        public Entity Room;
    }
}