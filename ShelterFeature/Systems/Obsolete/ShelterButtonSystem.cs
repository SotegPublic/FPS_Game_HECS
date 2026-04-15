using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.Shelter, Doc.UI, "shelter button system")]
    public sealed class ShelterButtonSystem : BaseSystem, IAfterEntityInit
    {
        [Required] private UIAccessProviderComponent accessProvider;

        public override void InitSystem()
        {
        }

        public void AfterEntityInit()
        {
            accessProvider.Get.GetButton(UIAccessIdentifierMap.ShelterButton).onClick.AddListener(OpenShelter);
        }

        private void OpenShelter()
        {
            Owner.World.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.LoadShelterSceneState });
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            accessProvider.Get.GetButton(UIAccessIdentifierMap.ShelterButton).onClick.RemoveListener(OpenShelter);
        }
    }
}