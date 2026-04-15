using HECSFramework.Core;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Camera, Doc.Config, "ShelterCameraConfigComponent")]
    public sealed class ShelterCameraConfigComponent : BaseComponent
    {
        [Header("Camera settings")]
        public float XOffset = -1;
        public float BaseCameraZCoordinate = -10f;
        public Vector2 TargetAspect = new Vector2(9, 16);

        [Header("Base move settings")]
        public float SwipeThreshold = 2f;
        public ShelterCameraMoveType MoveType = ShelterCameraMoveType.MoveByRooms;

        [Header("Move on rooms settings")]
        [ShowIf("isMoveByRooms")]
        public float CameraOffset = 0.6f;
        [ShowIf("isMoveByRooms")]
        public float MoveByRoomsCameraSpeed = 0.2f;
        [ShowIf("isMoveByRooms")]
        public float ChangeRoomThreshold = 0.2f;

        [Header("Free move settings")]
        [ShowIf("isHybridOrFreeMove")]
        public float FreeMoveCameraSpeed = 40f;

        [Header("Hybrid move settings")]
        [ShowIf("isHybridMove")]
        public float HybridSwipeThreshold = 4f;

        public float SqrSwipeThreshold => SwipeThreshold * SwipeThreshold;
        public float SqrChangeRoomThreshold => ChangeRoomThreshold * ChangeRoomThreshold;

        private bool isMoveByRooms => MoveType == ShelterCameraMoveType.MoveByRooms;
        private bool isHybridMove => MoveType == ShelterCameraMoveType.Hybrid;
        private bool isHybridOrFreeMove => MoveType == ShelterCameraMoveType.Hybrid || MoveType == ShelterCameraMoveType.FreeMove;
    }

    public enum ShelterCameraMoveType
    {
        None = 0,
        MoveByRooms = 1,
        FreeMove = 2,
        Hybrid = 3
    }

    public enum DirectionTypes
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }
}