using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Helpers;
using System.Buffers;

namespace Systems
{
    [Serializable][Documentation(Doc.DrawRule, Doc.Visual, "this system draw visual when player get resources to position")]
    public sealed class ResourceToPositionDrawRuleSystem : BaseSystem, IReactCommand<DrawGlobalCounterRewardCommand>, IUpdatable
    {
        [Required]
        private DrawRuleTagComponent ruleTagComponent;
        [Required]
        private DrawRuleTargetPositionComponent targetPositionComponent;
        [Required]
        private ViewReferenceGameObjectComponent viewReferenceGameObjectComponent;
        [Required]
        private VFXDrawResourceConfigComponent config;
        [Required]
        private ResourceToPositionDrawRuleStateComponent stateComponent;

        [Single]
        private PoolingSystem poolingSystem;

        public override void InitSystem()
        {
        }

        public async void CommandReact(DrawGlobalCounterRewardCommand command)
        {
            if (ruleTagComponent.CounterIdentifierContainers != command.GlobalResourceRewardCommand.CounterID ||
                ruleTagComponent.DrawRuleIdentifiers != command.GlobalResourceRewardCommand.DrawRule)
                return;

            command.GlobalResourceRewardCommand.From.Entity.GetOrAddComponent<VisualLocalLockComponent>().AddLock();

            var particleSystemView = await poolingSystem.GetViewFromPool(viewReferenceGameObjectComponent.ViewReference);

            var particleSystem = particleSystemView.GetComponent<ParticlesProviderMonoComponent>().ParticleSystem;

            var drone = Owner.World.GetEntityBySingleComponent<LooterDroneTagComponent>();
            var rewardEntity = command.GlobalResourceRewardCommand.From.Entity;
            var fx = drone.GetComponent<AssetRefIDHolderComponent>().GetRef(AssetRefIDMap.GatheringGlow);
            var gatheringGlowCommand = new AddLineRenderFXGlobalCommand
            {
                AssetReference = fx,
                EffectGuid = Guid.NewGuid(),
                From = drone.GetComponent<UnityTransformComponent>(),
                To = rewardEntity.GetComponent<UnityTransformComponent>(),
            };

            Owner.World.Command(gatheringGlowCommand);

            var emission = particleSystem.emission;
            emission.enabled = false;

            var module = particleSystem.main;
            module.simulationSpace = ParticleSystemSimulationSpace.Local;

            var neededTransform = command.GlobalResourceRewardCommand.From.Entity.GetTransform();
            particleSystemView.transform.SetPositionAndRotation(neededTransform.position, neededTransform.rotation);

            var particlesCountToEmit = command.GlobalResourceRewardCommand.Amount > config.SpawnParticlesCount ?
                config.SpawnParticlesCount : command.GlobalResourceRewardCommand.Amount;

            particleSystem.Play();
            particleSystem.Emit(particlesCountToEmit);

            var context = new ResourceToPositionParticleSystemContext
            {
                CounterID = command.GlobalResourceRewardCommand.CounterID,
                Owner = command.GlobalResourceRewardCommand.From,
                To = command.GlobalResourceRewardCommand.To,
                TargetPosition = particleSystem.transform.InverseTransformPoint(targetPositionComponent.TargetPosition),
                TotalRewardAmount = command.GlobalResourceRewardCommand.Amount,
                CurrentParticlesCount = particlesCountToEmit,
                FirstStatePercent = config.NonControlStatePercentage,
                ParticleSystem = particleSystem,
                ParticleView = particleSystemView,
                ParticleSystemGravityModifier = particleSystem.main.gravityModifier.constant,

                Particles = HECSPooledArray<ParticleSystem.Particle>.GetArray(particlesCountToEmit).Items,
                ParticleContexts = HECSPooledArray<ParticleContext>.GetArray(particlesCountToEmit).Items,
                ParticleRewards = HECSPooledArray<int>.GetArray(particlesCountToEmit).Items,

                GatherGlowGuid = gatheringGlowCommand.EffectGuid,
                NextRewardIndex = 0,
                LastDiviationCurvesCollectionIndex = -1,
            };

            var particlesCount = particleSystem.GetParticles(context.Particles);

            InitParticlesContexts(ref context, particlesCount);

            CalculateRewardByParticle(ref context);

            stateComponent.ResourceParticleContexts.Add(context);
            particleSystem.SetParticles(context.Particles, particlesCount);
        }

        private void InitParticlesContexts(ref ResourceToPositionParticleSystemContext context, int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                context.Particles[i].randomSeed = (uint)i;
                ref var particleContext = ref context.ParticleContexts[context.Particles[i].randomSeed];

                particleContext.StartPosition = context.Particles[i].position;
                particleContext.IsInControlState = false;
                particleContext.DiviationCurveIndex = GetDiviationCurveIndex(ref context);
                particleContext.RemainingLifeTime = context.Particles[i].startLifetime;
            }
        }

        private int GetDiviationCurveIndex(ref ResourceToPositionParticleSystemContext context)
        {
            var lenth = config.DeviationCurvesCollections.Length;
            var lastIndex = lenth - 1;

            if (lenth > 1)
            {
                var index = UnityEngine.Random.Range(0, lenth);

                if (index == context.LastDiviationCurvesCollectionIndex)
                {
                    if (index + 1 > lastIndex)
                    {
                        index--;
                    }
                    else
                    {
                        index++;
                    }
                }

                context.LastDiviationCurvesCollectionIndex = index;
                return index;
            }

            return lastIndex;
        }

        private void CalculateRewardByParticle(ref ResourceToPositionParticleSystemContext context)
        {
            var mod = context.TotalRewardAmount % context.CurrentParticlesCount;
            var rewardAmount = (context.TotalRewardAmount - mod) / context.CurrentParticlesCount;

            var stepsWithGreaterReward = context.TotalRewardAmount - (context.CurrentParticlesCount * rewardAmount);
            for (int i = 0; i < context.CurrentParticlesCount; i++)
            {
                if (i < stepsWithGreaterReward)
                {
                    context.ParticleRewards[i] = rewardAmount + 1;
                }
                else
                {
                    context.ParticleRewards[i] = rewardAmount;
                }
            }
        }

        public void UpdateLocal()
        {
            for (int i = 0; i < stateComponent.ResourceParticleContexts.Count; i++)
            {
                ref var currentContext = ref stateComponent.ResourceParticleContexts[i];

                var particlesCount = currentContext.ParticleSystem.GetParticles(currentContext.Particles);

                UpdateRemainingLifeTime(ref currentContext, particlesCount);
                MoveParticles(ref currentContext, particlesCount);

                currentContext.ParticleSystem.SetParticles(currentContext.Particles, particlesCount);


                SendReward(ref currentContext);

                CheckIsAllParticlesDead(ref currentContext);
            }

            ClearContextCollection(stateComponent.ResourceParticleContexts);
        }

        private void UpdateRemainingLifeTime(ref ResourceToPositionParticleSystemContext currentContext, int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                ref var particle = ref currentContext.Particles[i];
                ref var particleContext = ref currentContext.ParticleContexts[particle.randomSeed];
                particleContext.RemainingLifeTime -= Time.deltaTime;

                if (!particleContext.IsInControlState)
                {
                    var progress = 1 - (particleContext.RemainingLifeTime / particle.startLifetime);

                    if (progress >= currentContext.FirstStatePercent)
                    {
                        particleContext.IsInControlState = true;
                        particleContext.StartPosition = particle.position;
                        particleContext.ControledStateLifeTime = particleContext.RemainingLifeTime;

                        particle.velocity = Vector3.zero;

                        var main = currentContext.ParticleSystem.main;
                        var gravityCurve = currentContext.ParticleSystem.main.gravityModifier;
                        gravityCurve.constant = 0;
                        main.gravityModifier = gravityCurve;
                    }
                }

                if(particleContext.RemainingLifeTime <= 0)
                {
                    particle.remainingLifetime = 0;
                }
            }
        }

        private void MoveParticles(ref ResourceToPositionParticleSystemContext currentContext, int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                ref var particle = ref currentContext.Particles[i];
                ref var particleContext = ref currentContext.ParticleContexts[particle.randomSeed];

                if (particleContext.IsInControlState)
                {
                    var finalStateProgress = 1 - (particleContext.RemainingLifeTime / particleContext.ControledStateLifeTime);

                    var lerpDirection = Vector3.LerpUnclamped(particleContext.StartPosition, currentContext.TargetPosition, finalStateProgress);

                    if (config.IsControledByCurves)
                    {
                        ChangeDirectionByCurves(finalStateProgress, ref lerpDirection,
                            config.DeviationCurvesCollections[particleContext.DiviationCurveIndex]);
                    }

                    particle.position = lerpDirection;
                }
            }
        }

        private void ChangeDirectionByCurves(float finalStateProgress, ref Vector3 lerpDirection, DeviationCurvesCollection diviationCurvesCollection)
        {
            lerpDirection.x *= diviationCurvesCollection.XDeviationCurve.Evaluate(finalStateProgress);
            lerpDirection.y *= diviationCurvesCollection.YDeviationCurve.Evaluate(finalStateProgress);
            lerpDirection.z *= diviationCurvesCollection.ZDeviationCurve.Evaluate(finalStateProgress);
        }

        private void SendReward(ref ResourceToPositionParticleSystemContext currentContext)
        {
            if (currentContext.CurrentParticlesCount > currentContext.ParticleSystem.particleCount)
            {
                var delta = currentContext.CurrentParticlesCount - currentContext.ParticleSystem.particleCount;

                for (int i = 0; i < delta; i++)
                {
                    var reward = currentContext.ParticleRewards[currentContext.NextRewardIndex];

                    Owner.World.Command(new UpdateVisualRewardCounterCommand
                    {
                        To = currentContext.To,
                        Amount = reward,
                        CounterID = currentContext.CounterID
                    });
                    currentContext.TotalRewardAmount -= reward;
                    currentContext.NextRewardIndex++;
                }

                currentContext.CurrentParticlesCount -= delta;
            }
        }

        private void CheckIsAllParticlesDead(ref ResourceToPositionParticleSystemContext context)
        {
            if (context.ParticleSystem.particleCount == 0)
            {
                context.IsNeedToRemove = true;

                if (context.TotalRewardAmount >= 0)
                {
                    Owner.World.Command(new UpdateVisualRewardCounterCommand
                    {
                        To = context.To,
                        Amount = context.TotalRewardAmount,
                        CounterID = context.CounterID
                    });
                    context.TotalRewardAmount = 0;
                    context.CurrentParticlesCount = 0;
                }
            }
        }

        private void ClearContextCollection(HECSList<ResourceToPositionParticleSystemContext> collection)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                ref var currentContext = ref collection[i];

                if (currentContext.IsNeedToRemove)
                {
                    ArrayPool<ParticleSystem.Particle>.Shared.Return(currentContext.Particles);
                    ArrayPool<ParticleContext>.Shared.Return(currentContext.ParticleContexts);
                    ArrayPool<int>.Shared.Return(currentContext.ParticleRewards);

                    var main = currentContext.ParticleSystem.main;
                    var gravityCurve = currentContext.ParticleSystem.main.gravityModifier;
                    gravityCurve.constant = currentContext.ParticleSystemGravityModifier;
                    main.gravityModifier = gravityCurve;

                    currentContext.ParticleSystem.Stop();

                    Owner.World.Command(new RemoveLineRenderFXGlobalCommand
                    {
                        EffectGuid = currentContext.GatherGlowGuid
                    });

                    currentContext.Owner.Entity.GetComponent<VisualLocalLockComponent>().Remove();

                    poolingSystem.ReleaseView(currentContext.ParticleView);

                    stateComponent.ResourceParticleContexts.RemoveAt(i);
                }
            }
        }
    }
}

public struct ResourceToPositionParticleSystemContext : IEquatable<ResourceToPositionParticleSystemContext>
{
    public int CounterID;
    public AliveEntity Owner;
    public AliveEntity To;
    public int TotalRewardAmount;
    public int[] ParticleRewards;
    public Vector3 TargetPosition;
    public Guid GatherGlowGuid;

    public ParticleSystem ParticleSystem;
    public ParticleSystem.Particle[] Particles;
    public ParticleContext[] ParticleContexts;
    public int CurrentParticlesCount;
    public GameObject ParticleView;
    public float ParticleSystemGravityModifier;

    public float FirstStatePercent;
    public bool IsNeedToRemove;
    public int NextRewardIndex;
    public int LastDiviationCurvesCollectionIndex;


    public bool Equals(ResourceToPositionParticleSystemContext other)
    {
        return other.ParticleSystem = this.ParticleSystem;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ParticleSystem);
    }
}

public struct ParticleContext
{
    public Vector3 StartPosition;
    public bool IsInControlState;
    public int DiviationCurveIndex;
    public float ControledStateLifeTime;
    public float RemainingLifeTime;
}