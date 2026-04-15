using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, "here we hold buff abilities")]
    public sealed class BuffAbilitiesHolderComponent : BaseContainerHolderComponent<BuffAbilityTagComponent>
    {
    }
}