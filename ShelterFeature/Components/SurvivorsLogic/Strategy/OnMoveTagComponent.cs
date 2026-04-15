using HECSFramework.Core;
using HECSFramework.Unity;
using Strategies;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, Doc.Strategy, "we use this tag for moving entity")]
    public sealed class OnMoveTagComponent : BaseComponent
    {
        public Vector3 TargetLocalPositionInRoom;
        public SurvivorDirection Direction;

        public override void BeforeDispose()
        {
            TargetLocalPositionInRoom = Vector3.zero;
            Direction = SurvivorDirection.None;
        }
    }

    public enum SurvivorDirection
    {
        None = 0,
        Left = 1,
        Right = 2,
    }
}