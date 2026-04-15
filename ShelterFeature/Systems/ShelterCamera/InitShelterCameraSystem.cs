using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Cysharp.Threading.Tasks;

namespace Systems
{
    [RequiredAtContainer(typeof(VirtualCameraProviderComponent))]
	[Serializable][Documentation(Doc.Shelter, Doc.Camera, "this system init camera on start")]
    public sealed class InitShelterCameraSystem : BaseSystem, IAfterEntityInit
    {
        [Required] private CameraFOVSettingsComponent cameraSettings;
        [Required] private UnityTransformComponent transformComponent;
        [Required] private ShelterCameraConfigComponent cameraConfig;

        public void AfterEntityInit()
        {
            var fov = cameraSettings.NonActiveFOV;
            FitingCamera();
            Owner.World.Request<UniTask, ChangeCameraFOVCommand>(
                new ChangeCameraFOVCommand { CameraIndex = CameraIdentifierMap.ShelterCamera, FOV = fov, IsAnimated = false }).Forget();
        }

        public override void InitSystem()
        {
        }

        private void FitingCamera()
        {
            var currentAspectModifier = (float)Screen.width / Screen.height;
            var targetAspectModifier = cameraConfig.TargetAspect.x / cameraConfig.TargetAspect.y; 
            var scaleModifier = targetAspectModifier / currentAspectModifier;

            transformComponent.Transform.position = new Vector3(0,0, cameraConfig.BaseCameraZCoordinate * scaleModifier);
        }
    }
}