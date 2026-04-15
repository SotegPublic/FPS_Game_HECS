using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.UI, "RoomServiceUISystemStateComponent")]
    public sealed class RoomServiceUISystemStateComponent : BaseComponent
    {
        public HECSList<UIAccessMonoComponent> JobIndicators = new HECSList<UIAccessMonoComponent>(8);
    }
}