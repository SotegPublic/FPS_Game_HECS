using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "shelter energyflow counter")]
    public sealed class ShelterEnergyflowCounterComponent : ModifiableIntCounterComponent
    {
        [SerializeField] private int energyflow;

        public override int Id => CounterIdentifierContainerMap.Energyflow;

        public override int SetupValue => energyflow;

        public override void Init()
        {
            base.Init();
            this.SetReactive(true);
        }
    }
}