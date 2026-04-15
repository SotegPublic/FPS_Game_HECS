using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "counter for reserved survivors")]
    public sealed class ReservedSurvivorsCounterComponent : SimpleIntCounterBaseComponent
    {
        [SerializeField] private int reservedSurvivors;

        public override int Value { get => reservedSurvivors; protected set => reservedSurvivors = value; }

        public override int Id => CounterIdentifierContainerMap.ReservedSurvivors;
    }
}