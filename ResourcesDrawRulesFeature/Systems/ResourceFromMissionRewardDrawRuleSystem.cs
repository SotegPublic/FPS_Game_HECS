using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.DrawRule, Doc.Visual, "this system draw visual when player get resources from mission rewards")]
    public sealed class ResourceFromMissionRewardDrawRuleSystem : BaseSystem, IReactCommand<DrawGlobalCounterRewardCommand>
    {
        [Required]
        private DrawRuleTagComponent ruleTagComponent;

        public override void InitSystem()
        {
        }

        public async void CommandReact(DrawGlobalCounterRewardCommand command)
        {
            if (ruleTagComponent.CounterIdentifierContainers != command.GlobalResourceRewardCommand.CounterID ||
                ruleTagComponent.DrawRuleIdentifiers != command.GlobalResourceRewardCommand.DrawRule)
                return;

            Owner.World.Command(new UpdateVisualRewardCounterCommand
            {
                To = command.GlobalResourceRewardCommand.To,
                Amount = command.GlobalResourceRewardCommand.Amount,
                CounterID = command.GlobalResourceRewardCommand.CounterID
            });
        }
    }
}
