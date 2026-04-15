using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Debuff, "BurnConfigComponent")]
    public sealed class BurnConfigComponent : BaseComponent
    {
        [SerializeField] private int maxStacksCount;

        public int MaxStacksCount => maxStacksCount;
    }
}