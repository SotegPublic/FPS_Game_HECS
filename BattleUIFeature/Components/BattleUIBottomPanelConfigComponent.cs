using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.UI, "here we set parameters for Battle UI Bottom Character Panel")]
    public sealed class BattleUIBottomPanelConfigComponent : BaseComponent
    {
        [SerializeField] private float lazyHealthDecreaseSpeed;

        public float LazyHealthDecreaseSpeed => lazyHealthDecreaseSpeed;
    }
}