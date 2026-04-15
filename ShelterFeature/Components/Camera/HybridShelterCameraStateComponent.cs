using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "HybridShelterCameraStateComponent")]
    public sealed class HybridShelterCameraStateComponent : BaseComponent
    {
        public bool IsInputActive;
        public bool IsChangeColumnInProgress;

        public Vector2 CurrentInputPosition;
        public Vector2 StartInputPosition;
        public Vector3 CameraStartPosition;

        public Vector2 VerticalEdge;

        public Vector3 Velocity;
        public float NewOffsetX;
        public Vector3 NewCameraPosition;
        public DirectionTypes CurrentDirection;
    }
}