using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Random = UnityEngine.Random;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Movement, "this system is responsible for the drone's tilt and yaw")]
    public sealed class DroneTiltAndBiasSystem : BaseViewSystem, ILateUpdatable
    {
        [Required] private DroneFollowVelocityComponent velocityComponent;
        [Required] private DroneTiltAndBiasConfigComponent config;
        [Required] private DroneTiltAndBiasStateComponent variables;

        public override void InitSystem()
        {
            variables.PerlinYOffset = Random.Range(0f, 100f);
        }

        public void UpdateLateLocal()
        {
            if (!isReady)
                return;

            var viewTransform = variables.View.transform;

            var lateralPerlinNoise = Mathf.PerlinNoise(Time.time * config.PerlinXSpeed, variables.PerlinYOffset) * 2 - 1;
            lateralPerlinNoise = Mathf.Lerp(variables.PreviousNoise, lateralPerlinNoise, config.PerlinSmoothModifier);
            variables.PreviousNoise = lateralPerlinNoise;

            var distanceToEdge = config.BiasEdge - Mathf.Abs(viewTransform.localPosition.x);
            var edgeSmoothFactor = Mathf.Clamp01(distanceToEdge / config.BiasSoftnessEdge);
            var addedBiasVector = Vector3.right * Mathf.Clamp(config.BiasIntensity * lateralPerlinNoise * edgeSmoothFactor, -config.BiasEdge, config.BiasEdge);

            viewTransform.localPosition = Vector3.Lerp(viewTransform.localPosition, variables.StartPosition + addedBiasVector, config.BiasSpeed * Time.deltaTime);
            var forwardTilt = Mathf.Clamp(velocityComponent.Velocity.magnitude * config.TiltModifier, 0, config.MaxForwardTilt);

            Quaternion targetTilt = Quaternion.Euler(forwardTilt, 0, -lateralPerlinNoise * config.MaxLateralTilt * edgeSmoothFactor);
            viewTransform.localRotation = Quaternion.Slerp(viewTransform.localRotation, targetTilt, config.TiltSpeed * Time.deltaTime);
        }

        protected override void InitAfterViewLocal()
        {
            variables.View = Owner.GetComponent<ViewReadyTagComponent>().View;
            variables.StartPosition = variables.View.transform.localPosition;
        }

        protected override void ResetLocal()
        {
        }
    }
}