using System;
using HECSFramework.Core;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Buff, Doc.Abilities, "config component for burn ability")]
    public sealed class BurnArtefactAbilityConfigComponent : BaseComponent
    {
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField][Range(0f, 1f)] private float procChance;
        [SerializeField] private float duration;
        [SerializeField] private float frequencyPerSecond;
        [SerializeField][Range(0f, 1f)] private float percentDamageValue;
        [SerializeField] private float showVFXTime;

        public float ProcChance => procChance;
        public float PercentDamageValue => percentDamageValue;
        public float Duration => duration;
        public float FrequencyPerSecond => frequencyPerSecond;
        public float ExplosionRadius => explosionRadius;
        public float ShowVFXTime => showVFXTime;

    }
}