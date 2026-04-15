using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Tag, "RoomsParrentTagComponent")]
    public sealed class RoomsParentTagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}