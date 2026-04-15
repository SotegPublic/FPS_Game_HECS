using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using Systems;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "Shelter Feature State Component")]
    public sealed class ShelterFeatureStateComponent : BaseComponent
    {
        public SceneInstance ShelterScene;
        public RoomCell[,] ShelterGrid;
    }
}