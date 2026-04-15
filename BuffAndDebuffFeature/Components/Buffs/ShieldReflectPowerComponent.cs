using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, "ShieldReflectPowerComponent")]
    public sealed class ShieldReflectPowerComponent : ModifiableFloatCounterComponent
    {
        [SerializeField] private float power;
        public override int Id => CounterIdentifierContainerMap.ShieldReflectPower;

        public override float SetupValue => power;
    }
}