using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, Doc.Tag, "by this tag we mark passive abilities that affect weapons")]
    public sealed class WeaponBuffTagComponent : BaseComponent
    {
    }
}