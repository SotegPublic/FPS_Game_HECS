using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using UnityEngine.Events;
using Commands;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.UI, Doc.Shelter, "this system init room world canvas")]
    public sealed class InitRoomUISystem : BaseSystem, IInitAfterView
    {
        [Required] private UIAccessProviderComponent accessProvider;
        [Required] private UIAccessPrfbProviderComponent prfbProvider;
        [Required] private CountersHolderComponent countersHolder;
        [Required] private RoomServiceConfigComponent roomServiceConfig;
        [Required] private RoomServiceUISystemStateComponent uISystemStateComponent;

        public void InitAfterView()
        {
            accessProvider.Get.ProcessTags();

            var raycastTarget = accessProvider.Get.GetGenericComponent<RoomRaycastTargetMonoComponent>(UIAccessIdentifierMap.RaycastTarget);
            raycastTarget.RoomActor = Owner.AsActor();

            var progressImage = accessProvider.Get.GetImage(UIAccessIdentifierMap.Progressbar);
            var progressCounter = countersHolder.GetOrAddFloatCounter(CounterIdentifierContainerMap.Progress);
            progressImage.fillAmount = progressCounter.Value;

            var camera = Owner.World.GetSingleComponent<MainCameraComponent>().Camera;
            var canvas = accessProvider.Get.GetGenericComponent<Canvas>(UIAccessIdentifierMap.Canvas);
            canvas.worldCamera = camera;

            var jobsIndicatorsRoot = accessProvider.Get.GetRectTransform(UIAccessIdentifierMap.Root);

            for(int i = 0; i < roomServiceConfig.MaxAwaitingServiceJobs; i++)
            {
                var jobIndicatorPref = prfbProvider.Get.GetPrefab(UIAccessIdentifierMap.RoomService);

                var jobIndicator = MonoBehaviour.Instantiate(jobIndicatorPref, jobsIndicatorsRoot);
                var monoComponent = jobIndicator.GetComponent<UIAccessMonoComponent>();

                uISystemStateComponent.JobIndicators.Add(monoComponent);
            }
        }

        public override void InitSystem()
        {
        }

        public void Reset()
        {
        }
    }
}