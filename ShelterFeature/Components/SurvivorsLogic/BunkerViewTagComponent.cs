using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Tag, "bunker tag for shelter scene")]
    public sealed class BunkerViewTagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}