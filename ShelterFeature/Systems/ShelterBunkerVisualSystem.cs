using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.Visual, "this system open and close bunker gates for survivors")]
    public sealed class ShelterBunkerVisualSystem : BaseSystem, IUpdatable, IGlobalStart
    {
        [Required] private AnimatorStateComponent animatorStateComponent;
        
        private ArrivedSurvivorsHolderComponent survivorsHolderComponent;
        private const float DistanceBeforeOpen = 5;
        private bool isGateOpened;

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            var isSurvivorNear = false;

            foreach(var arrivedSurvivors in survivorsHolderComponent.Survivors)
            {
                for(int i = 0; i < arrivedSurvivors.Value.Count; i++)
                {
                    var survivor = arrivedSurvivors.Value[i];

                    if(survivor.IsInBunker)
                        continue;

                    if (Vector3.Distance(survivor.Entity.GetPosition(), Owner.GetPosition()) < DistanceBeforeOpen)
                    {
                        isSurvivorNear = true;
                        break;
                    }
                }

                if(isSurvivorNear)
                    break;
            }

            if (isSurvivorNear && !isGateOpened)
            {
                Owner.Command(new BoolAnimationCommand { Index = AnimParametersMap.Open, Value = true });
                isGateOpened = true;
            }
            else if (!isSurvivorNear && isGateOpened)
            {
                var currentState = animatorStateComponent.Animator.GetCurrentAnimatorStateInfo(0);
                bool isPlaying = currentState.normalizedTime < 1.0f;

                if (!isPlaying)
                {
                    Owner.Command(new BoolAnimationCommand { Index = AnimParametersMap.Open, Value = false });
                    isGateOpened = false;
                }
            }
        }

        public void GlobalStart()
        {
            survivorsHolderComponent = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>().GetComponent<ArrivedSurvivorsHolderComponent>();
        }
    }
}