using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using UnityEngine.UI;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, Doc.Character, "BattleUICharacterPanelSystem")]
    public sealed class BattleUICharacterPanelSystem : BaseSystem, IUpdatable, IReactGlobalCommand<UpdateHealthOnUICommand>
    {
        [Required] private UIAccessProviderComponent uiAccess;
        [Required] private BattleUIBottomPanelConfigComponent config;

        private bool isHealthDecrease;
        private float newFillValue;

        public void CommandGlobalReact(UpdateHealthOnUICommand command)
        {
            var healthImage = uiAccess.Get.GetImage(UIAccessIdentifierMap.Health);
            newFillValue = Mathf.Clamp(command.CurrentHP, 0, command.MaxHP) / command.MaxHP;

            healthImage.fillAmount = newFillValue;
            isHealthDecrease = true;
        }

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            if(isHealthDecrease)
            {
                var lazyHealthBar = uiAccess.Get.GetImage(UIAccessIdentifierMap.HealthLazy);
                var decreaseValue = Time.deltaTime * config.LazyHealthDecreaseSpeed;

                lazyHealthBar.fillAmount = Mathf.Max(lazyHealthBar.fillAmount - decreaseValue, newFillValue); 

                if (lazyHealthBar.fillAmount == newFillValue)
                    isHealthDecrease = false;
            }
        }
    }
}

namespace Commands
{
    [Documentation(Doc.UI, "by this command we update health on UI")]
    public struct UpdateHealthOnUICommand : IGlobalCommand
    {
        public float MaxHP;
        public float CurrentHP;
    }
}