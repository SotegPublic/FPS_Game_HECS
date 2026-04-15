using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "here we hold info about survivor target room")]
    public sealed class SurvivorRoomHolderComponent : BaseComponent
    {
        public Entity Room;

        public override void BeforeDispose()
        {
            Room = null;
        }
    }
}