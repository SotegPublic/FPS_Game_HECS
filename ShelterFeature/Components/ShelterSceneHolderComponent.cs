using HECSFramework.Core;
using System;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Holder, "here we hold shelter scene asset reference")]
    public sealed class ShelterSceneHolderComponent : BaseComponent
    {
        public AssetReference SceneReference;
    }
}