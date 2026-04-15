using System;
using HECSFramework.Core;
using Systems;

namespace Components
{
    [Serializable][Documentation(Doc.DrawRule, "YellowCrystalsFromNodeDrawRuleStateComponent")]
    public class YellowCrystalsFromNodeDrawRuleStateComponent : BaseComponent
    {
        public HECSList<YellowCrystalsFromNodeDrawRuleSystemContext> YellowCrystalsDrawRuleContexts = new HECSList<YellowCrystalsFromNodeDrawRuleSystemContext>(4);
    }
}