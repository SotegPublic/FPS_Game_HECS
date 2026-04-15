using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.UI, Doc.Tag, "we mark battle ui by this tag")]
    public sealed class BattleUITagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}