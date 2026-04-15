using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Helpers;
using BluePrints.Identifiers;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, "we use this tag for buff passive abilities")]
    public sealed class BuffAbilityTagComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(BuffIdentifier))] private int buffID;

        public int BuffID => buffID;
    }
}