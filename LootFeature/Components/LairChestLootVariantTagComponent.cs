using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Loot, "we use this tag to distinguish lair chest drop containers from other containers.")]
    public sealed class LairChestLootVariantTagComponent : BaseComponent
    {
    }

    [Serializable]
    [Documentation(Doc.Loot, "we use this tag to distinguish enemy drop containers from other containers.")]
    public sealed class EnemyLootVariantTagComponent : BaseComponent
    {
    }
}