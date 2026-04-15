using System;
using System.Buffers;
using System.Threading.Tasks;
using Commands;
using Components;
using Components.MonoBehaviourComponents;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.DrawRule, "YellowCrystalsFromNodeDrawRuleSystem")]
    public sealed class YellowCrystalsFromNodeDrawRuleSystem : BaseSystem, IReactCommand<DrawGlobalCounterRewardCommand>, IUpdatable
    {
        [Required]
        private DrawRuleTagComponent ruleTagComponent;
        [Required]
        private YellowCrystalsFromNodeDrawRuleStateComponent stateComponent;
        [Required]
        public ActionsHolderComponent ActionsHolder;

        public void CommandReact(DrawGlobalCounterRewardCommand command)
        {
            DrawAsync(command).Forget();
        }

        private async UniTask DrawAsync(DrawGlobalCounterRewardCommand command)
        {
            if (ruleTagComponent.CounterIdentifierContainers != command.GlobalResourceRewardCommand.CounterID ||
                ruleTagComponent.DrawRuleIdentifiers != command.GlobalResourceRewardCommand.DrawRule)
                return;

            var nodeMonoComponent = command.GlobalResourceRewardCommand.From.Entity.AsActor().GetComponent<YellowCrystalsNodeMonoComponent>();
            var drone = Owner.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
            drone.GetOrAddComponent<VisualLocalLockComponent>().AddLock();

            var fx = drone.GetComponent<AssetRefIDHolderComponent>().GetRef(AssetRefIDMap.GatheringGlow);
            var gatheringGlowCommand = new AddLineRenderFXGlobalCommand
            {
                AssetReference = fx,
                EffectGuid = Guid.NewGuid(),
                From = drone.GetComponent<UnityTransformComponent>(),
                To = nodeMonoComponent,
            };

            Owner.World.Command(gatheringGlowCommand);
            ActionsHolder.ExecuteAction(ActionIdentifierMap.VisualStrategy, command.GlobalResourceRewardCommand.From);

            await new WaitFor<ReadyToCollectTagComponent>(command.GlobalResourceRewardCommand.From.Entity).RunJob();

            var context = new YellowCrystalsFromNodeDrawRuleSystemContext
            {
                CounterID = command.GlobalResourceRewardCommand.CounterID,
                From = command.GlobalResourceRewardCommand.From,
                To = command.GlobalResourceRewardCommand.To,
                RewardAmount = command.GlobalResourceRewardCommand.Amount,
                GatheringGlowCommandGuid = gatheringGlowCommand.EffectGuid
            };

            stateComponent.YellowCrystalsDrawRuleContexts.Add(context);

            var droneCollectStrength = drone.GetComponent<DroneCollectWhileFollowConfigComponent>().CollectStrength;
            var durability = command.GlobalResourceRewardCommand.From.Entity.GetComponent<NodeDurabilityComponent>().Durability;
            var duration = durability / droneCollectStrength;

            command.GlobalResourceRewardCommand.From.Entity.Command(new StartCollectNodeCommand 
            { 
                CollectTo = drone.GetTransform(),
                Duration = duration
            });
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            for(int i = 0; i < stateComponent.YellowCrystalsDrawRuleContexts.Count; i++)
            {
                ref var currentContext = ref stateComponent.YellowCrystalsDrawRuleContexts[i];

                if (currentContext.IsNeedToRemove)
                    continue;
                
                if(currentContext.From.Entity.ContainsMask<EndCollectTagComponent>())
                {
                    SendReward(ref currentContext);
                }
            }

            ClearContextCollection(stateComponent.YellowCrystalsDrawRuleContexts);
        }

        private void SendReward(ref YellowCrystalsFromNodeDrawRuleSystemContext currentContext)
        {
            Owner.World.Command(new UpdateVisualRewardCounterCommand
            {
                To = currentContext.To,
                Amount = currentContext.RewardAmount,
                CounterID = currentContext.CounterID
            });

            currentContext.From.Entity.AddComponent<NodeCollectedTagComponent>();
            currentContext.IsNeedToRemove = true;
        }

        private void ClearContextCollection(HECSList<YellowCrystalsFromNodeDrawRuleSystemContext> collection)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                ref var currentContext = ref collection[i];

                if (!currentContext.IsNeedToRemove)
                    continue;

                Owner.World.Command(new RemoveLineRenderFXGlobalCommand
                {
                    EffectGuid = currentContext.GatheringGlowCommandGuid
                });

                currentContext.From.Entity.Command(new IsDeadCommand());

                var drone = Owner.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
                drone.GetOrAddComponent<VisualLocalLockComponent>().Remove();

                stateComponent.YellowCrystalsDrawRuleContexts.RemoveAt(i);
            }
        }
    }

    public struct YellowCrystalsFromNodeDrawRuleSystemContext
    {
        public int CounterID;
        public AliveEntity From;
        public AliveEntity To;
        public int RewardAmount;
        public Guid GatheringGlowCommandGuid;
        public bool IsNeedToRemove;
    }
}