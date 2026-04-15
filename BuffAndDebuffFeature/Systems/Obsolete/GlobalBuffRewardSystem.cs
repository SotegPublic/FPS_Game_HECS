using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Item, Doc.Rewards, "this system procces buffs rewards and execute visual scenarios for buffs")]
    public sealed class GlobalBuffRewardSystem : BaseSystem, IReactGlobalCommand<GlobalBuffRewardCommand>
    {
        [Required]
        private DrawRuleEntitiesHolderComponent drawRuleEntitiesHolderComponent;

        public void CommandGlobalReact(GlobalBuffRewardCommand command)
        {
            var drawCommand = new DrawBuffRewardCommand
            {
                GlobalBuffRewardCommand = command
            };

            foreach (var ruleEntity in drawRuleEntitiesHolderComponent.DrawRulesGenericEntites)
            {
                ruleEntity.Command(drawCommand);
            }
        }

        public override void InitSystem()
        {
        }
    }
}