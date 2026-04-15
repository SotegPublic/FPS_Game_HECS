using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, "ShieldRechargePowerComponent")]
    public sealed class ShieldRechargePowerComponent : ModifiableFloatCounterComponent
    {
        [SerializeField] private float rechargePower;
        public override int Id => CounterIdentifierContainerMap.ShieldRechargePower;

        public override float SetupValue => rechargePower;

        public override void Init()
        {
            base.Init();
            SetReactive(true);
        }
    }
}