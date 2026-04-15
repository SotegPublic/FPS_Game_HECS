using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, "ShieldVampiricPowerComponent")]
    public sealed class ShieldVampiricPowerComponent : ModifiableFloatCounterComponent
    {
        [SerializeField] private float power;
        public override int Id => CounterIdentifierContainerMap.ShieldVampiricPower;

        public override float SetupValue => power;
    }
}