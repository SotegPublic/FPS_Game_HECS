using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.DrawRule, "ResourceToBackpackDrawRuleStateComponent")]
    public sealed class ResourceToPositionDrawRuleStateComponent : BaseComponent
    {
        public HECSList<ResourceToPositionParticleSystemContext> ResourceParticleContexts = new HECSList<ResourceToPositionParticleSystemContext>(4);
    }
}