using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Cheats, Doc.Tag, "we set this tag when start with cheats")]
    public sealed class CheatStartTagComponent : BaseComponent
    {
    }
}