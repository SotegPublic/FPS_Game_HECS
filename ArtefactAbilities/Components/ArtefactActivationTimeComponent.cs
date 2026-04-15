using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Artefact, Doc.Visual, "ArtefactActivationTimeComponent")]
    public sealed class ArtefactActivationTimeComponent : BaseComponent
    {
        [SerializeField] private float activationTime;

        public float ActivationTime => activationTime;
    }
}