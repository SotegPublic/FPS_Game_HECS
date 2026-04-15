using Cinemachine;
using Commands;
using Components;
using Cysharp.Threading.Tasks;
using Systems;
using UnityEngine;

namespace HECSFramework.Core.ShooterMiniGameStates
{
    [Documentation(Doc.ShooterFeature, "this state start game raid")]
    public class PrepareForCheatStartState : ShooterState
    {
        public override int StateID { get; } = ShooterState.PrepareForCheatStartState;
        public PrepareForCheatStartState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
        {
        }

        public override void Enter(Entity entity)
        {
            EnterAsync(entity).Forget();
        }

        private async UniTask EnterAsync(Entity entity)
        {
            var cheatFeature = entity.World.GetEntityBySingleComponent<CheatFeatureTagComponent>();
            var cheatStartParameters = cheatFeature.GetComponent<CheatStartParametersComponent>();

            await EntityManager.Default.GetSingleSystem<UISystem>().ShowUIGroup(new UIGroupCommand()
            {
                UIGroup = UIGroupIdentifierMap.ShooterUIGroupIdentifier,
                Show = true
            });

            entity.World.Command(new ActivateWeaponsCommand());
            entity.World.Command(new ActivatePlayerItemsCommand());
            entity.World.Command(new InitUIOnRaidStartCommand());

            var raidState = entity.World.GetSingleComponent<ShooterZoneStateComponent>();

            if (cheatStartParameters.StartShootingPoint.ContainsMask<EnterZoneTagComponent>())
            {
                var zoneView = entity.World.GetFilter<EnterZoneViewTagComponent>().FirstOrDefault(x => x.GetComponent<EnterZoneViewTagComponent>().ZoneIndex == raidState.EnterID);

                if (zoneView != null)
                {
                    zoneView.Command(new BoolAnimationCommand { Index = AnimParametersMap.Open, Value = true });
                }
                else
                    Debug.LogError("we dont have view for enter zone " + IdentifierToStringMap.IntToString[raidState.EnterID]);

                zoneView.GetComponent<AnimationCheckOutsHolderComponent>().TryGetCheckoutInfo(AnimationEventIdentifierMap.Open, out var info);
                await UniTask.Delay(info.ClipLenght.ToMilliseconds());
            }

            var zones = entity.World.GetFilter<ShooterZoneTagComponent>();
            zones.ForceUpdateFilter();

            raidState.CurrentShootingPoint = cheatStartParameters.StartShootingPoint.GetAliveEntity();
            raidState.ZoneIndexInProgress = cheatStartParameters.TargetZoneIndex;
            raidState.CurrentZone = zones.FirstOrDefault(x => x.GetComponent<ZoneIndexComponent>().Index == cheatStartParameters.TargetZoneIndex).GetAliveEntity();
            raidState.CurrentRaidStep = CalculateSteps(cheatStartParameters, entity, raidState);

            var mainChar = EntityManager.Default.GetSingleComponent<MainCharacterTagComponent>().Owner;

            RotateMainCharToShootingPoint(entity, raidState, mainChar);
            await SetCameraFollow(mainChar);
            await entity.World.Request<UniTask, PrepareForShootingCommand>(new PrepareForShootingCommand { OwnerIndex = mainChar.Index });

            entity.World.Command(new HideUICommand { UIViewType = UIIdentifierMap.LoadingScreen_UIIdentifier });
            cheatFeature.RemoveComponent<CheatStartTagComponent>();

            EndState();
        }

        private int CalculateSteps(CheatStartParametersComponent cheatStartParameters, Entity entity, ShooterZoneStateComponent raidState)
        {
            var raidManager = entity.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            var mission = raidManager.GetComponent<MissionsConfigsHolderComponent>().GetMissionByID(cheatStartParameters.MissionID);
            var missionPath = mission.PathNodes; 

            var steps = 0;

            for (int i = 0; i < missionPath.Length; i++)
            {
                if(cheatStartParameters.TargetZoneIndex != missionPath[i])
                    steps++;
                else 
                    break;
            }

            return steps; 
        }

        private void RotateMainCharToShootingPoint(Entity entity, ShooterZoneStateComponent raidState, Entity mainChar)
        {
            var mainCharPosition = mainChar.GetPosition();
            var nextShootingPoint = raidState.CurrentShootingPoint.Entity.GetPosition();
            var directionToNextShootingPoint = nextShootingPoint - mainCharPosition;

            directionToNextShootingPoint.y = 0f;

            var rotationOfCharacter = Quaternion.LookRotation(directionToNextShootingPoint, Vector3.up);
            mainChar.GetTransform().rotation = rotationOfCharacter;
            entity.World.Command(new ForceVirtualCameraFollowAndLookAtCommand { CameraId = CameraIdentifierMap.ShooterRunningCameraIdentifier });
            entity.World.Command(new ForceFinishCinemachineBrainBlendCommand());
        }

        private async UniTask SetCameraFollow(Entity entity)
        {
            var virtualCamera = GetVirtualCamera(CameraIdentifierMap.ShooterCharacterCameraIdentifier);
            var transitionCamera = GetVirtualCamera(CameraIdentifierMap.ShooterTransitionCameraIdentifier);
            var runningVirtualCamera = GetVirtualCamera(CameraIdentifierMap.ShooterRunningCameraIdentifier);
            var character = entity.World.GetEntityBySingleComponent<MainCharacterTagComponent>();

            await new WaitFor<ViewReadyTagComponent>(character).RunJob();

            var cameraTargetsProvider = character.GetComponent<CameraTargetProviderComponent>();
            var aimTargetProvider = character.GetComponent<CharacterAimTargetComponent>();
            virtualCamera.Follow = cameraTargetsProvider.GetTarget(CameraIdentifierMap.ShooterCharacterCameraIdentifier).transform;
            virtualCamera.LookAt = aimTargetProvider.AimTarget;
            transitionCamera.Follow = cameraTargetsProvider.GetTarget(CameraIdentifierMap.ShooterTransitionCameraIdentifier).transform;
            transitionCamera.LookAt = cameraTargetsProvider.GetTarget(CameraIdentifierMap.ShooterTransitionCameraIdentifier).transform;
            runningVirtualCamera.Follow = cameraTargetsProvider.GetTarget(CameraIdentifierMap.ShooterRunningCameraIdentifier).transform;
            runningVirtualCamera.LookAt = cameraTargetsProvider.GetTarget(CameraIdentifierMap.ShooterRunningCameraIdentifier).transform;

            entity.World.Command(new FocusCameraByIdCommand { CameraId = CameraIdentifierMap.ShooterRunningCameraIdentifier });
            entity.World.Command(new ForceVirtualCameraFollowAndLookAtCommand { CameraId = CameraIdentifierMap.ShooterRunningCameraIdentifier });
            entity.World.Command(new ForceFinishCinemachineBrainBlendCommand());

            await UniTask.Delay(500);
        }

        private CinemachineVirtualCamera GetVirtualCamera(int cameraId)
        {
            var cameras = EntityManager.Default.GetFilter(Filter.Get<VirtualCameraProviderComponent>());
            cameras.ForceUpdateFilter();

            var cameraEntity = cameras.FirstOrDefault(a =>
            {
                var cameraIdentifier = a.GetComponent<VirtualCameraComponent>().CameraId;
                return cameraIdentifier != null &&
                       cameraIdentifier.Id == cameraId;
            });

            var provider = cameraEntity?.GetComponent<VirtualCameraProviderComponent>();
            return provider?.Get;
        }

        public override void Exit(Entity entity)
        {
        }

        public override void Update(Entity entity)
        {
        }
    }
}