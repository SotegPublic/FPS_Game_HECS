using HECSFramework.Core;
using System;
using Systems;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "MoveByRoomsShelterCameraStateComponent")]
    public sealed class MoveByRoomsShelterCameraStateComponent : BaseComponent
    {
        public bool IsInputActive;
        public bool IsChangeRoomInProgress;

        public Vector2 CurrentInputPosition;
        public Vector2 StartInputPosition;
        
        public Vector3 Velocity;
        public Vector3 NewOffsetPosition;
        public Vector3 NewCameraPosition;
        public DirectionTypes CurrentDirection;
    }
}