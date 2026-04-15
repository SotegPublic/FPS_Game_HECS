using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Tag, Doc.Loot, "mark loot entity after calculating drop rewards")]
    public sealed class LootCalculatedTagComponent : BaseComponent
    {
    }
}