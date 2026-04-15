using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Counters, Doc.Shelter, "shelter cashflow config")]
    public sealed class CashflowConfig : BaseComponent
    {
        [SerializeField] private int tickTime;

        public int TickTime => tickTime;
    }
}