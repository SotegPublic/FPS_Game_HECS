using System;
using Commands;
using Components;
using HECSFramework.Core;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.Player, Doc.Visual, Doc.Shield, "this system show and hide player shield")]
    public sealed class ShieldVisualSystem : BaseSystem, IInitAfterView, IUpdatable, 
        IReactCommand<AfterCommand<DamageCommand<float>>>, 
        IReactGlobalCommand<UpdateBackpackPositionCommand>, 
        IReactGlobalCommand<AfterCommand<DismantleAfterShootingCommand>>,
        IReactGlobalCommand<AnimateShieldCommand>
    {
        [Required] private LayersComponent layers;
        [Required] private ShieldObjectProviderComponent shieldObjectProvider;
        [Required] private ShieldVisualSystemComponent systemComponent;
        private Material shieldMat;
        private float alpha;

        public void CommandGlobalReact(UpdateBackpackPositionCommand command)
        {
            Owner.Command(new BoolAnimationCommand { Value = true, Index = AnimParametersMap.Open });
        }

        public void CommandGlobalReact(AfterCommand<DismantleAfterShootingCommand> command)
        {
            Owner.Command(new BoolAnimationCommand { Value = false, Index = AnimParametersMap.Open });
        }

        public void CommandReact(AfterCommand<DamageCommand<float>> command)
        {
            if (command.Value.DamageData.DamageKeeper != Owner)
                return;

            StartAnimation();
        }

        private void StartAnimation()
        {
            systemComponent.CurrentShownProgress = systemComponent.ShownTime;
            systemComponent.IsHidden = false;
            shieldObjectProvider.Get.gameObject.layer = layers.ShowLayer.LayerID;
        }

        public void CommandGlobalReact(AnimateShieldCommand command)
        {
            StartAnimation();
        }

        public void InitAfterView()
        {
            shieldMat = shieldObjectProvider.Get.gameObject.GetComponent<MeshRenderer>().material;
            alpha = shieldMat.color.a;
        }

        public override void InitSystem()
        {
            systemComponent.IsHidden = true;
        }

        public void Reset()
        {
        }

        public void UpdateLocal()
        {
            if (systemComponent.IsHidden)
                return;

            systemComponent.CurrentShownProgress -= Time.deltaTime;

            if(systemComponent.CurrentShownProgress < 0)
            {
                systemComponent.IsHidden = true;
                systemComponent.CurrentShownProgress = 0;
                shieldObjectProvider.Get.gameObject.layer = layers.HideLayer.LayerID;
                return;
            }

            var currentColor = shieldMat.color;
            currentColor.a =  alpha * (systemComponent.CurrentShownProgress / systemComponent.ShownTime);
            shieldMat.color = currentColor;
        }
    }
}

namespace Commands
{
    public struct AnimateShieldCommand : IGlobalCommand
    {
        public Color Color;
    }
}