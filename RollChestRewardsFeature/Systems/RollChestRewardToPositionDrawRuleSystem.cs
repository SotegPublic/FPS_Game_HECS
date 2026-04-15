using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Random = UnityEngine.Random;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.DrawRule, Doc.Loot, Doc.Item, "this system draw visual when player get item to position")]
    public sealed class RollChestRewardToPositionDrawRuleSystem : BaseSystem, IReactCommand<DrawItemRewardCommand>, IUpdatable
    {
        [Required]
        private DrawRuleTagComponent ruleTagComponent;
        [Required]
        private DrawRuleTargetPositionComponent targetPositionComponent;
        [Required]
        private VFXDrawLootItemsConfigComponent config;
        [Required]
        private RewardItemToPositionDrawRuleStateComponent stateComponent;

        [Single] private PoolingSystem pool;

        public void CommandReact(DrawItemRewardCommand command)
        {
            if (ruleTagComponent.DrawRuleIdentifiers != command.GlobalItemRewardCommand.DrawRule)
                return;

            command.GlobalItemRewardCommand.From.Entity.GetOrAddComponent<VisualLocalLockComponent>().AddLock();

            var drone = Owner.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
            var rewardEntity = command.GlobalItemRewardCommand.RewardView.GetComponent<Actor>().Entity;
            var fx = drone.GetComponent<AssetRefIDHolderComponent>().GetRef(AssetRefIDMap.GatheringGlow);
            var gatheringGlowCommand = new AddLineRenderFXGlobalCommand
            {
                AssetReference = fx,
                EffectGuid = Guid.NewGuid(),
                From = drone.GetComponent<UnityTransformComponent>(),
                To = rewardEntity.GetComponent<UnityTransformComponent>(),
            };

            Owner.World.Command(gatheringGlowCommand);

            var context = new RewardItemToPositionSystemContext
            {
                Owner = command.GlobalItemRewardCommand.From,
                TargetPosition = targetPositionComponent.TargetPosition,
                StartPosition = command.GlobalItemRewardCommand.RewardView.transform.position,
                ItemView = command.GlobalItemRewardCommand.RewardView,
                RewardItemID = command.GlobalItemRewardCommand.RewardItemID,
                ItemsCount = command.GlobalItemRewardCommand.Amount,
                Progress = 0,
                DeviationCurveID = config.IsControledByCurves ? Random.Range(0, config.DeviationCurvesCollections.Length) : -1,
                GatherGlowGuid = gatheringGlowCommand.EffectGuid
            };

            stateComponent.RewardItemContexts.Add(context);
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            for (int i = 0; i < stateComponent.RewardItemContexts.Count; i++)
            {
                ref var context = ref stateComponent.RewardItemContexts[i];
                context.Progress += Time.deltaTime * config.Speed;

                MoveView(ref context);
                ScaleView(ref context);

                if (context.Progress >= 1)
                {
                    AddItemToInventory(context);
                    context.IsNeedToRemove = true;
                }
            }

            RemoveEndedContexts();
        }

        private void AddItemToInventory(RewardItemToPositionSystemContext context)
        {
            var raidManager = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            raidManager.Command(new AddChestRewardsCommand
            {
                chestEntity = context.Owner
            });
        }

        private void MoveView(ref RewardItemToPositionSystemContext context)
        {
            var newPosition = Vector3.LerpUnclamped(context.StartPosition, context.TargetPosition, context.Progress);

            if (config.IsControledByCurves)
            {
                ChangeDirectionByCurves(context.Progress, ref newPosition,
                    config.DeviationCurvesCollections[context.DeviationCurveID]);
            }

            context.ItemView.transform.position = newPosition;
        }

        private void ScaleView(ref RewardItemToPositionSystemContext context)
        {
            context.ItemView.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, context.Progress);
        }

        private void ChangeDirectionByCurves(float progress, ref Vector3 newPosition, DeviationCurvesCollection diviationCurvesCollection)
        {
            newPosition.x *= diviationCurvesCollection.XDeviationCurve.Evaluate(progress);
            newPosition.y *= diviationCurvesCollection.YDeviationCurve.Evaluate(progress);
            newPosition.z *= diviationCurvesCollection.ZDeviationCurve.Evaluate(progress);
        }

        private void RemoveEndedContexts()
        {
            for (int i = stateComponent.RewardItemContexts.Count; i >= 0; i--)
            {
                ref var context = ref stateComponent.RewardItemContexts[i];

                if (!context.IsNeedToRemove)
                    continue;

                Owner.World.Command(new RemoveLineRenderFXGlobalCommand
                {
                    EffectGuid = context.GatherGlowGuid
                });

                context.ItemView.SetActive(false);
                context.ItemView.transform.localScale = Vector3.one;

                pool.ReleaseView(context.ItemView);
                context.Owner.Entity.GetOrAddComponent<VisualLocalLockComponent>().Remove();

                stateComponent.RewardItemContexts.RemoveAt(i);
            }
        }
    }

    public struct RewardItemToPositionSystemContext : IEquatable<RewardItemToPositionSystemContext>
    {
        public AliveEntity Owner;
        public Vector3 TargetPosition;
        public Vector3 StartPosition;
        public GameObject ItemView;
        public int RewardItemID;
        public int ItemsCount;
        public float Progress;
        public int DeviationCurveID;
        public Guid GatherGlowGuid;
        public bool IsNeedToRemove;

        public bool Equals(RewardItemToPositionSystemContext other)
        {
            return other.ItemView.GetInstanceID() == this.ItemView.GetInstanceID();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ItemView.GetInstanceID());
        }
    }
}