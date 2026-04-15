using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Survivros, Doc.Shelter, "this system update survivors count for storage")]
    public sealed class SurvivorsStorageSystem : BaseSystem, IReactGlobalCommand<ShelterDataWasLoadedCommand>, IReactCommand<DiffCounterCommand<int>>
    {
        [Required] public SurvivorsStorageCounterComponent storageComponent;

        private bool isSystemActive;

        public void CommandGlobalReact(ShelterDataWasLoadedCommand command)
        {
            isSystemActive = true;
        }

        public void CommandReact(DiffCounterCommand<int> command)
        {
            if (!isSystemActive)
                return;

            if(command.Id != CounterIdentifierContainerMap.Survivors)
                return;

            storageComponent.SetValue(command.Value);
        }

        public override void InitSystem()
        {
        }
    }
}