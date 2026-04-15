using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.UI, "BattleUIBuffSystemStateComponent")]
    public sealed class BattleUIBuffSystemStateComponent : BaseComponent
    {
        public Dictionary<Guid, AbilityIconMonoComponent> Icons = new Dictionary<Guid, AbilityIconMonoComponent>(16);
    }
}