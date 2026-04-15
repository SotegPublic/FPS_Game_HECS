using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.Config, "config for lightning arc FX")]
    public sealed class LightningArcFXConfigComponent : BaseComponent
    {
        [SerializeField] private float showTime;

        public float ShowTime => showTime;
    }
}