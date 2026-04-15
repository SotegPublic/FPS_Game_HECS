using System;
using HECSFramework.Core;
using UnityEngine;

namespace Components
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Buff, Doc.Abilities, "config component for lightning arc ability")]
    public sealed class LightningArcPassiveAbilityConfigComponent : BaseComponent
    {
        [SerializeField][Range(0f, 1f)] private float procChance;
        [SerializeField] private float radius;
        [SerializeField][Range(0f, 1f)] private float percentDamageValue;
        [SerializeField] private float fxShowTime;

        public float ProcChance => procChance;
        public float Radius => radius;
        public float PercentDamageValue => percentDamageValue;
        public float FXShowTime => fxShowTime;
    }
}