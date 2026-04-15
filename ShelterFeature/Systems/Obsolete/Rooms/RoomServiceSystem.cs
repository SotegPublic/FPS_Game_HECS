using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.Shelter, "this system execute service")]
    public sealed class RoomServiceSystem : BaseSystem, IUpdatable
    {
        [Required] private RoomServiceConfigComponent serviceConfig;
        [Required] private CountersHolderComponent countersHolder;
        [Required] private RoomServiceSystemStateComponent systemState;

        private ShelterRoomsJobsHolderComponent jobsHolder;

        public void CompleteJob(ICounter<float> progressCounter, int jobsCount)
        {
            Owner.Command(new UpdateRoomUIWhileJobProgressCommand { Progress = 0f });
            Owner.Command(new UpdateRoomUIJobsIndicators { IsJobComplete = true, IndicatorIndex = jobsCount});

            for (int i = 0; i < serviceConfig.ResourcesForService.Length; i++)
            {
                var resourceConfig = serviceConfig.ResourcesForService[i];
                var resourceCounter = countersHolder.GetOrAddIntCounter(resourceConfig.ResourceID);

                resourceCounter.ChangeValue(-resourceConfig.ServiceCost);
            }

            progressCounter.SetValue(0);
            systemState.CurrentJob = null;
            Owner.RemoveComponent<RoomExecuteServiceTagComponent>();
        }

        public override void InitSystem()
        {
            jobsHolder = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>().GetComponent<ShelterRoomsJobsHolderComponent>();
        }

        public void UpdateLocal()
        {
            if (!Owner.ContainsMask<RoomExecuteServiceTagComponent>())
            {
                var jobQueue = jobsHolder.GetJobsQueue(Owner.GUID);
                if (IsServiceCanStart() && jobQueue.TryDequeue(out var job))
                {
                    systemState.CurrentJob = job;
                    Owner.GetOrAddComponent<RoomExecuteServiceTagComponent>();
                }
            }
            else
            {
                var progressCounter = countersHolder.GetOrAddFloatCounter(CounterIdentifierContainerMap.Progress);
                var addingProgress = Time.deltaTime / serviceConfig.ServiceDuration;
                progressCounter.ChangeValue(addingProgress);
                Owner.Command(new UpdateRoomUIWhileJobProgressCommand { Progress = progressCounter.Value });

                if (progressCounter.Value >= 1)
                {
                    var survivor = systemState.CurrentJob.Survivor;
                    survivor.Command(new ForceStopAICommand());
                    survivor.HecsDestroy();
                    CompleteJob(progressCounter, jobsHolder.GetJobsQueueCount(Owner.GUID));
                }
            }
        }

        private bool IsServiceCanStart()
        {
            var isServiceCanStart = true;

            for (int i = 0; i < serviceConfig.ResourcesForService.Length; i++)
            {
                var resourceConfig = serviceConfig.ResourcesForService[i];
                var counter = countersHolder.GetOrAddIntCounter(resourceConfig.ResourceID);

                if (resourceConfig.ServiceCost > counter.Value)
                    isServiceCanStart = false;
            }

            return isServiceCanStart;
        }
    }
}