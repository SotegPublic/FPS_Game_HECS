using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "ElevatorStateComponent")]
    public sealed class ElevatorSystemStateComponent : BaseComponent
    {
        public Vector3 ElevatorBasePosition;
        public float ElevatorProgress;
    }
}