using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, "CreateRoomPopupUISystem")]
    public sealed class CreateRoomPopupUISystem : BaseSystem, IAfterEntityInit
    {
        [Required] private UIAccessProviderComponent uiAccessProvider;
        [Required] private UIAccessPrfbProviderComponent prfbProviderComponent;

        private ContentSizeFitter filter;
        private Button addRoomButton;
        private UIAccessMonoComponent shelterUIAccess;
        private HECSList<ButtonInfo> createRoomButtons = new HECSList<ButtonInfo>(12);
        private Entity shelterFeature;

        public override void InitSystem()
        {
        }

        public void AfterEntityInit()
        {
            shelterUIAccess = uiAccessProvider.Get.GetUIAccess(UIAccessIdentifierMap.ShelterUI);
            shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();

            HidePopup();
            var shelter = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();
            var comp = shelter.GetComponent<NewRoomPanelStateComponent>();
            addRoomButton = shelter.GetComponent<NewRoomPanelStateComponent>().
                AddNewRoomComponent.AddButton;

            var closePopupBtn = shelterUIAccess.GetButton(UIAccessIdentifierMap.CreateRoomExitButton);
            closePopupBtn.onClick.AddListener(HidePopup);

            addRoomButton.onClick.AddListener(ShowPopup);

            FillPopupWindow();
        }

        private void FillPopupWindow()
        {
            var availableRooms = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<AvailableRoomsTypesComponent>();
            var root = shelterUIAccess.GetRectTransform(UIAccessIdentifierMap.CreateRoom);
            var pref = prfbProviderComponent.Get.GetPrefab(UIAccessIdentifierMap.CreateRoom);

            foreach(var roomType in availableRooms.AvailableRooms)
            {
                var roomButton = MonoBehaviour.Instantiate(pref, root);
                var roomButtonUIAccess = roomButton.GetComponent<UIAccessMonoComponent>();

                var nameText = roomButtonUIAccess.GetTextMeshProUGUI(UIAccessIdentifierMap.Text);
                var cost = roomButtonUIAccess.GetTextMeshProUGUI(UIAccessIdentifierMap.Cost);
                var button = roomButtonUIAccess.GetButton(UIAccessIdentifierMap.Button);

                nameText.text = IdentifierToStringMap.IntToString[roomType];

                createRoomButtons.Add(new ButtonInfo { Button = button, ButtonId = roomType});
                button.onClick.AddListener(() =>
                {
                    Owner.World.Command(new CreateRoomCommand { RoomTypeID = roomType });
                    HidePopup();
                });
            }
        }

        private void ShowPopup()
        {
            shelterFeature.GetOrAddComponent<BlockShelterSystemTagComponent>();

            var createRoomCanvasGroup = shelterUIAccess.GetCanvasGroup(UIAccessIdentifierMap.CreateRoom);
            createRoomCanvasGroup.alpha = 1;
            createRoomCanvasGroup.blocksRaycasts = true;
            //var popupRect = uiAccessProvider.Get.GetRectTransform(UIAccessIdentifierMap.CreateRoomPopupRoot);
            //PositionPopupNearCursor(popupRect);
        }

        //private void PositionPopupNearCursor(RectTransform popupRect)
        //{
        //    Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        //    var mainCanvas = Owner.World.GetEntityBySingleComponent<MainCanvasTagComponent>().AsActor().GetComponent<Canvas>();
        //    var canvasRect = mainCanvas.GetComponent<RectTransform>();

        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouseScreenPosition, null, out var localPosition);

        //    var popupSize = popupRect.rect.size;
        //    var canvasHalfWidth = canvasRect.rect.width * 0.5f;
        //    var canvasHalfHeight = canvasRect.rect.height * 0.5f;

        //    //simple
        //    //var targetPositionX = Mathf.Clamp(localPosition.x, -canvasHalfWidth, canvasHalfWidth - popupSize.x);
        //    //var targetPositionY = Mathf.Clamp(localPosition.y, -canvasHalfHeight + popupSize.y, canvasHalfHeight);

        //    var fitsRight = localPosition.x + popupSize.x <= canvasHalfWidth;
        //    var fitsDown = localPosition.y - popupSize.y >= -canvasHalfHeight;

        //    var targetPositionX = localPosition.x - (fitsRight ? 0 : popupSize.x);
        //    var targetPositionY = localPosition.y + (fitsDown ? 0 : popupSize.y);

        //    var targetPosition = new Vector2(targetPositionX, targetPositionY);

        //    var screenPosition = RectTransformUtility.WorldToScreenPoint(null, canvasRect.TransformPoint(targetPosition));

        //    popupRect.position = screenPosition;
        //}

        private void HidePopup()
        {
            var createRoomCanvasGroup = shelterUIAccess.GetCanvasGroup(UIAccessIdentifierMap.CreateRoom);
            createRoomCanvasGroup.alpha = 0;
            createRoomCanvasGroup.blocksRaycasts = false;

            shelterFeature.RemoveComponent<BlockShelterSystemTagComponent>();
        }

        public override void BeforeDispose()
        {
            var closePopupBtn = shelterUIAccess.GetButton(UIAccessIdentifierMap.CreateRoomExitButton);
            closePopupBtn.onClick.RemoveListener(HidePopup);

            addRoomButton.onClick.RemoveListener(ShowPopup);

            foreach(var button in createRoomButtons)
            {
                button.Button.onClick.RemoveAllListeners();
            }

            createRoomButtons.ClearFast();
            shelterUIAccess = null;
        }
    }

    public class ButtonInfo
    {
        public Button Button;
        public int ButtonId;
    }
}