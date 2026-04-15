using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "shelter energyflow config")]
    public sealed class EnergyflowConfig : BaseComponent
    {
        [SerializeField] private int tickTime;

        public int TickTime => tickTime;
    }
}