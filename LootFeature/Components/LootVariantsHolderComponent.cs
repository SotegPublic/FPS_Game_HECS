using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Loot, Doc.Holder, "here we hold loot variants containers")]
    public sealed class LootVariantsHolderComponent : BaseContainerHolderComponent<LootVariantTagComponent>
    {
    }
}