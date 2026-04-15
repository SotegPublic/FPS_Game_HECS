using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.Shelter, Doc.UI, "this system control all ui about room service")]
    public sealed class RoomServiceUISystem : BaseSystem, IReactCommand<UpdateRoomUIWhileJobProgressCommand>, IReactCommand<UpdateRoomUIJobsIndicators>
    {
        [Required] private UIAccessProviderComponent accessProvider;
        [Required] private RoomServiceUISystemStateComponent systemState;

        public void CommandReact(UpdateRoomUIWhileJobProgressCommand command)
        {
            if (!Owner.ContainsMask<ViewReadyTagComponent>())
                return;

            var progressImage = accessProvider.Get.GetImage(UIAccessIdentifierMap.Progressbar);

            progressImage.fillAmount = command.Progress;
        }

        public void CommandReact(UpdateRoomUIJobsIndicators command)
        {
            UpdateIndicatorAsync(command).Forget();
        }

        private async UniTask UpdateIndicatorAsync(UpdateRoomUIJobsIndicators command)
        {
            await new WaitFor<ViewReadyTagComponent>(Owner).RunJob();

            var indicator = systemState.JobIndicators[command.IndicatorIndex];
            indicator.GetImage(UIAccessIdentifierMap.RoomService).enabled = command.IsJobComplete ? false : true;
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Shelter, "UpdateRoomUIWhileJobProgressCommand")]
    public struct UpdateRoomUIWhileJobProgressCommand : ICommand
    {
        public float Progress;
    }

    [Obsolete]
    [Serializable]
    [Documentation(Doc.Shelter, "UpdateRoomUIAfterJobComplete")]
    public struct UpdateRoomUIJobsIndicators : ICommand
    {
        public int IndicatorIndex;
        public bool IsJobComplete;
    }
}