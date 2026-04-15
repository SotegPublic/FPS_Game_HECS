using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Helpers;
using BluePrints.Identifiers;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, Doc.Abilities, "here we configurate buff ability")]
    public sealed class BuffAbilityConfigComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(CounterIdentifierContainer))] int modifiableCounter;
        [SerializeField] private ModifierCalculationType calculationType;
        [SerializeField] private ModifierValueType modifierType;
        [SerializeField] private ValuesByGrade[] modifierValues;

        public int ModifiableCounter => modifiableCounter;
        public ModifierCalculationType CalculationType => calculationType;
        public ModifierValueType ModifierType => modifierType;

        public float GetValueByGrade(int gradeID)
        {
            foreach(var modifierValue in modifierValues)
            {
                if (modifierValue.Grade == gradeID)
                    return modifierValue.Value;
            }

            return 0f;
        }
    }

    [Serializable]
    public class ValuesByGrade
    {
        [IdentifierDropDown(nameof(GradeIdentifier))] public int Grade;
        public float Value;
    }


}