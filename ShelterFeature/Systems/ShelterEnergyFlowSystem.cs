using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Counters, "this system generate energy for shelter")]
    public sealed class ShelterEnergyFlowSystem : BaseSystem, IUpdatable
    {
        [Required] private ShelterEnergyflowCounterComponent EnergyflowCounterComponent;
        [Required] private EnergyflowConfig Config;
        
        private ShelterEnergyCounterComponent energyCounter;

        private float currentTime;

        public override void InitSystem()
        {
            energyCounter = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ShelterEnergyCounterComponent>();
        }

        public void UpdateLocal()
        {
            currentTime += Time.deltaTime;

            if(currentTime >= Config.TickTime)
            {
                var oldValue = energyCounter.Value;
                energyCounter.ChangeValue(EnergyflowCounterComponent.Value);
                Owner.World.Command(new UpdateShelterUICounterCommand
                {
                    CounterID = energyCounter.Id,
                    Value = energyCounter.Value,
                    OldValue = oldValue,
                    MaxValue = energyCounter.CalculatedMaxValue
                });

                currentTime -= Config.TickTime;
            }
        }
    }
}
