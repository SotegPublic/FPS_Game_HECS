using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "shelter cashflow counter")]
    public sealed class ShelterCashflowCounterComponent : ModifiableIntCounterComponent
    {
        [SerializeField] private int cashflow;

        public override int Id => CounterIdentifierContainerMap.Cashflow;

        public override int SetupValue => cashflow;

        public override void Init()
        {
            base.Init();
            this.SetReactive(true);
        }
    }
}