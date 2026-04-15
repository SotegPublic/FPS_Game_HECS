using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Buff, "BuffsAndDebuffsFeatureTagComponent")]
    public sealed class BuffsAndDebuffsFeatureTagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}