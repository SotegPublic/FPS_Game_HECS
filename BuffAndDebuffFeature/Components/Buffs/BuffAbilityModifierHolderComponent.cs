using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, Doc.Modifiers, "here we hold buff modifier")]
    public sealed class BuffAbilityModifierHolderComponent : BaseComponent
    {
        public DefaultFloatModifier Modifier;
    }
}