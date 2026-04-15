using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "room modifiers")]
    public sealed class RoomModifiersHolder : BaseComponent
    {
        [SerializeField] private ModifierConfig[] modifiers;

        public ModifierConfig[] Modifiers => modifiers;
    }

    [Serializable]
    public sealed class ModifierConfig
    {
        public int Value;
        public ModifierCalculationType CalculationType;
        public ModifierValueType ModifierType;
        [IdentifierDropDown(nameof(CounterIdentifierContainer))]public int ModifierCounterID;
    }
}