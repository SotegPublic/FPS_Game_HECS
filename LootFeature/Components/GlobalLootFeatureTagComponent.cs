using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Loot, Doc.Tag, "GlobalLootFeatureTagComponent")]
    public sealed class GlobalLootFeatureTagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}