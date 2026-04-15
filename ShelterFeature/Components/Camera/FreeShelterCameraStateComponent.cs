using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "FreeShelterCameraStateComponent")]
    public sealed class FreeShelterCameraStateComponent : BaseComponent
    {
        public Vector2 StartInputPosition;
        public Vector3 CameraStartPosition;

        public Vector2 HorizontalEdge;
        public Vector2 VerticalEdge;
    }
}