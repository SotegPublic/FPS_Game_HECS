using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.Shelter, "RoomServiceSystemStateComponent")]
    public sealed class RoomServiceSystemStateComponent : BaseComponent
    {
        public RoomServiceJob CurrentJob;
    }
}