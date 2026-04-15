using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Rewards, "here we hold target position for ToPosition draw rules")]
    public sealed class DrawRuleTargetPositionComponent : BaseComponent
    {
        public Transform TargetTransform; 
        public Vector3 TargetPosition => TargetTransform.position;
    }
}