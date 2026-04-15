using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Counters, "this system generate money for shelter")]
    public sealed class ShelterCashFlowSystem : BaseSystem, IUpdatable
    {
        [Required] private ShelterCashflowCounterComponent CashflowCounterComponent;
        [Required] private CashflowConfig Config;

        private float currentTime;

        public override void InitSystem()
        {
        }

        public void UpdateLocal()
        {
            currentTime += Time.deltaTime;

            if (currentTime >= Config.TickTime)
            {
                var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
                var counter = player.GetComponent<MoneyCounterComponent>();

                var oldValue = counter.Value;
                counter.ChangeValue(CashflowCounterComponent.Value);

                Owner.World.Command(new UpdateShelterUICounterCommand
                {
                    CounterID = counter.Id,
                    Value = counter.Value,
                    OldValue = oldValue,
                    MaxValue = 0
                });

                currentTime -= Config.TickTime;
            }
        }
    }
}
