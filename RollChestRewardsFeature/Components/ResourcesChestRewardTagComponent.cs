using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Rewards, Doc.Visual, "by this tag we mark rewards for filtring in drop configs")]
    public sealed class ResourcesChestRewardTagComponent : BaseComponent
    {
    }
}