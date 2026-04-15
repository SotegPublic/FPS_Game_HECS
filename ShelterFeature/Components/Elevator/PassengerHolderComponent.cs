using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "here we hold current passenger in elevator")]
    public sealed class PassengerHolderComponent : BaseComponent
    {
        public ArrivedSurvivor Passenger;
        public Entity TargetRoom;

        public void Clear()
        {
            Passenger = null;
            TargetRoom = null;
        }

        public override void BeforeDispose()
        {
            Clear();
        }
    }
}