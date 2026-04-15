using Components;
using HECSFramework.Core;
using UnityEngine;


namespace Strategies
{
    [Documentation(Doc.Strategy, "SetAwaitTimeNode")]
    public class SetAwaitTimeNode : InterDecision
    {
        public override string TitleOfNode { get; } = "SetAwaitTimeNode";

        protected override void Run(Entity entity)
        {
            var config = entity.GetComponent<BehavioralStrategyConfigComponent>();
            var cooldownTime = Random.Range(0f, config.MaxAwaitingTime);

            var tag = entity.AddComponent<OnCooldownTagComponent>();
            tag.Cooldown = cooldownTime;

            Next.Execute(entity);
        }
    }
}
