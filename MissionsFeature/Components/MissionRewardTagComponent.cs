using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using Systems;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Missions, Doc.Tag, "we use this tag for missions rewards")]
    public sealed class MissionRewardTagComponent : BaseComponent
    {
    }
}