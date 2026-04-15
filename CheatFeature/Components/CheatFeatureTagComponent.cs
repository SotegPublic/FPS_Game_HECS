using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, Doc.Cheats, Doc.Feature, "CheatFeatureTagComponent")]
    public sealed class CheatFeatureTagComponent : BaseComponent, IWorldSingleComponent
    {
       
    }
}