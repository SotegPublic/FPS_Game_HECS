using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, "ShieldRechargeChargesComponent")]
    public sealed class ShieldRechargeChargesComponent : ModifiableFloatCounterComponent
    {
        [SerializeField] private float charges;
        public override int Id => CounterIdentifierContainerMap.ShieldRechargeCharge;

        public override float SetupValue => charges;

        public override void Init()
        {
            base.Init();
            SetReactive(true);
        }
    }
}