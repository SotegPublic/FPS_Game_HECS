using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "shelter energy counter")]
    public sealed class ShelterEnergyCounterComponent : ModifiableIntCounterComponent
    {
        [SerializeField] private int energy;

        public override int Id => CounterIdentifierContainerMap.ShelterEnergy;

        public override int SetupValue => energy;

        public override void Init()
        {
            base.Init();
            this.SetReactive(true);
        }
    }
}