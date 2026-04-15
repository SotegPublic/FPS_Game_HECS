using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Tag, "ShelterFeatureTagComponent")]
    public sealed class ShelterFeatureTagComponent : BaseComponent, IWorldSingleComponent
    {
       
    }
}