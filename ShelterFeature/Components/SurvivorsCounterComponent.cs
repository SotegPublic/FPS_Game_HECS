using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "counter for survivors in room or shelter")]
    public sealed class SurvivorsCounterComponent : ModifiableIntCounterComponent
    {
        [SerializeField] private int survivors;

        public override int Id => CounterIdentifierContainerMap.Survivors;

        public override int SetupValue => survivors;

        public override void Init()
        {
            base.Init();
            this.SetReactive(true);
        }
    }
}