using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable]
    [Documentation(Doc.UI, "BattleUIResourcesSystemStateComponent")]
    public sealed class BattleUIResourcesSystemStateComponent : BaseComponent
    {
        public Dictionary<int, ResourceIconMonoComponent> Icons = new Dictionary<int, ResourceIconMonoComponent>(16);
    }
}