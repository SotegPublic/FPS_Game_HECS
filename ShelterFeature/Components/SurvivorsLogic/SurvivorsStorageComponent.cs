using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Survivros, "here we hold survivors count for save")]
    public sealed class SurvivorsStorageCounterComponent : SimpleIntCounterBaseComponent
    {
        [SerializeField] private int survivorsCount;

        public override int Value { get => survivorsCount; protected set => survivorsCount = value; }

        public override int Id => CounterIdentifierContainerMap.SurvivorsStorage;
    }
}