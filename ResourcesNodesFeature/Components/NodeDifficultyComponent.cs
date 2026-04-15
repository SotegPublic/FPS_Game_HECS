using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Resources, "here we set resources node grade")]
    public sealed class NodeDifficultyComponent : BaseComponent
    {
        public int NodeDifficulty;
    }
}