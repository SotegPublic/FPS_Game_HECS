using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "ElevatorConfigComponent")]
    public sealed class ElevatorConfigComponent : BaseComponent
    {
        [SerializeField] private float elevatorSpeed;
        [SerializeField] private float awaitTime;
        [SerializeField] private float elevatorFloorOffset;

        public float ElevatorSpeed => elevatorSpeed;
        public float AwaitTime => awaitTime;
        public float ElevatorFloorOffset => elevatorFloorOffset;
    }
}