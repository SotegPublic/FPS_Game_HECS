using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Tag, Doc.Strategy, "here we set parameters for strategy")]
    public sealed class BehavioralStrategyConfigComponent : BaseComponent
    {
        [SerializeField] private float maxAwaitingTime;

        public float MaxAwaitingTime => maxAwaitingTime;
    }
}