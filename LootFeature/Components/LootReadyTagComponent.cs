using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, Doc.Loot, "we add this tag when loot ready to get it")]
    public sealed class LootReadyTagComponent : BaseComponent
    {
    }
}