using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable][Documentation(Doc.Shelter, "this system init room")]
    public sealed class InitRoomSystem : BaseSystem, IAfterEntityInit
    {
        [Required] public CountersHolderComponent CountersHolder;
        [Required] private RoomModifiersHolder roomModifiersHolder;

        public void AfterEntityInit()
        {
            for(int i = 0; i < roomModifiersHolder.Modifiers.Length; i++)
            {
                var config = roomModifiersHolder.Modifiers[i];
                var modifier = new DefaultIntModifier
                {
                    ModifierCounterID = config.ModifierCounterID,
                    GetCalculationType = config.CalculationType,
                    GetModifierType = config.ModifierType,
                    GetValue = config.Value,
                    ModifierGuid = Owner.GUID,
                };
                
                var command = new AddCounterModifierCommand<int>
                {
                    Id = modifier.ModifierCounterID,
                    IsUnique = false,
                    Modifier = modifier,
                    Owner = Owner.GUID
                };

                if (CountersHolder.TryGetIntCounter(modifier.ModifierCounterID, out var counter))
                {
                    var currentValue = counter.Value;

                    Owner.Command(command);
                    counter.SetValue(currentValue);
                }

                UpdateCounters(command);
            }
        }

        private void UpdateCounters(AddCounterModifierCommand<int> command)
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var shelter = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();

            UpdateCountersOnEntity(command, player);
            UpdateCountersOnEntity(command, shelter);
        }

        private static void UpdateCountersOnEntity(AddCounterModifierCommand<int> command, Entity entity)
        {
            var countersHolder = entity.GetComponent<CountersHolderComponent>();

            if (countersHolder.TryGetIntCounter(command.Id, out var counter))
            {
                var currentValue = counter.Value;
                entity.Command(command);

                if (counter.Id != CounterIdentifierContainerMap.Cashflow && counter.Id != CounterIdentifierContainerMap.Energyflow)
                    counter.SetValue(currentValue);
            }
        }

        public override void InitSystem()
        {
        }
    }
}