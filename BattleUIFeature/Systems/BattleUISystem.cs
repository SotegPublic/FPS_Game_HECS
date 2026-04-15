using System;
using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine.SceneManagement;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, "BattleUISystem")]
    public sealed class BattleUISystem : BaseSystem, IAfterEntityInit, IReactGlobalCommand<NextZoneCommand>
    {
        [Required] private UIAccessProviderComponent uiAccessProvider;

        private ShooterZoneStateComponent shooterZoneStateComponent;

        public void AfterEntityInit()
        {
            uiAccessProvider.Get.GetButton(UIAccessIdentifierMap.Settings).onClick.AddListener(ClickSettingsReact);

            UpdateRouteText(shooterZoneStateComponent.ZoneIndexInProgress);
        }

        public override void InitSystem()
        {
            shooterZoneStateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
        }

        private void ClickSettingsReact()
        {
            SceneManager.LoadScene("Root");
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            uiAccessProvider.Get.GetButton(UIAccessIdentifierMap.Settings).onClick.RemoveListener(ClickSettingsReact);
        }

        public void CommandGlobalReact(NextZoneCommand command)
        {
            UpdateRouteText(command.ZoneID);
        }

        private void UpdateRouteText(int zoneID)
        {
            var neededZone = Owner.World.GetFilter<ZoneIndexComponent>().FirstOrDefault(x => x.GetComponent<ZoneIndexComponent>().Index == zoneID);
            var zoneName = neededZone.GetComponent<NameComponent>().DefaultName;

            var routeText = uiAccessProvider.Get.GetTextMeshProUGUI(UIAccessIdentifierMap.RouteText);
            routeText.text = zoneName;
        }
    }
}