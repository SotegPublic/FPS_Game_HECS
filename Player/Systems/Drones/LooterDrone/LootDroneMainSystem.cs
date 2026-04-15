using System;
using HECSFramework.Core;
using Components;
using Commands;
using static AwaitLootState;
using static TryFindAnotherLootState;
using static FollowState;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Loot, "This system is responsible for the looter drone behavior")]
    public sealed class LootDroneMainSystem : BaseSystem, IUpdatable
    {
        [Required] private CurrentLootTargetComponent currentLootTargetComponent;
        [Required] private RotationSpeedWhenCollectLootComponent rotationSpeedComponent;
        [Required] private UnityTransformComponent transformComponent;


        private StateMachine stateMachine;


        public override void InitSystem()
        {
            var lootFilter = Owner.World.GetFilter(Filter.Get<LootReadyTagComponent>());
            var nodesFilter = Owner.World.GetFilter(Filter.Get<ResourcesNodeTagComponent>());

            stateMachine = new StateMachine(Owner, true);
            stateMachine.AddTransition(LooterDroneState.AwaitLootState, new IsNeedToFollow(Owner.World, LooterDroneState.FollowState));
            stateMachine.AddTransition(LooterDroneState.TryFindAnotherLootState, new IsNoLootInZone(LooterDroneState.RotateToStartPositionState, lootFilter));
            stateMachine.AddTransition(LooterDroneState.FollowState, new IsResourcesNodeNear(LooterDroneState.GoToCollectState, Owner));
            stateMachine.AddTransition(LooterDroneState.RotateToStartPositionState, new IsDroneNearCharacter(Owner.World, LooterDroneState.RotateOnStartPositionState));

            stateMachine.AddState(new FollowState(stateMachine, LooterDroneState.AwaitLootState));

            stateMachine.AddState(new GoToCollectState(stateMachine, LooterDroneState.CollectState));
            stateMachine.AddState(new CollectState(stateMachine, nodesFilter, LooterDroneState.FollowState));

            stateMachine.AddState(new AwaitLootState(stateMachine, LooterDroneState.GoToLootState, lootFilter, currentLootTargetComponent));
            stateMachine.AddState(new GoToLootState(stateMachine, LooterDroneState.GetLootState));
            stateMachine.AddState(new GetLootState(stateMachine, lootFilter, LooterDroneState.TryFindAnotherLootState));
            stateMachine.AddState(new TryFindAnotherLootState(stateMachine, LooterDroneState.RotateToLootState, lootFilter, currentLootTargetComponent));
            stateMachine.AddState(new RotateToLootState(stateMachine, LooterDroneState.GoToLootState, transformComponent, rotationSpeedComponent));

            stateMachine.AddState(new RotateToStartPositionState(stateMachine, LooterDroneState.ReturnState, transformComponent, rotationSpeedComponent));
            stateMachine.AddState(new ReturnState(stateMachine, LooterDroneState.RotateOnStartPositionState));
            stateMachine.AddState(new RotateOnStartPositionState(stateMachine, LooterDroneState.AwaitLootState, transformComponent, rotationSpeedComponent));


            Owner.World.GetSingleComponent<CoreFSMsComponent>().LooterDrone = stateMachine;

            StartFollow();
        }

        private void StartFollow()
        {
            stateMachine.ChangeState(LooterDroneState.FollowState);
        }

        public void UpdateLocal()
        {
            stateMachine.UpdateLocal();
        }
    }
}