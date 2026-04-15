using HECSFramework.Core;
using System;
using Systems;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "here we hold roomCell which camera is looking at")]
    public sealed class ShelterCameraTargetComponent : BaseComponent
    {
        public RoomCell TargetCell;
    }
}