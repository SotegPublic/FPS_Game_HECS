using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.Counters, "this system send commands when shelter counters updates")]
    public sealed class ShelterCountersUpdateSystem : BaseSystem, IReactCommand<DiffCounterCommand<int>>
    {
        [Required] public ShelterUICountersFilter countersFilter;
        public void CommandReact(DiffCounterCommand<int> command)
        {
            if (!countersFilter.ContainCounter(command.Id))
                return;

            Owner.World.Command(new UpdateShelterUICounterCommand
            {
                Value = command.Value,
                MaxValue = command.MaxValue,
                OldValue = command.PreviousValue,
                CounterID = command.Id
            });
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shelter, Doc.Counters, "by this command we update shelter ui counters")]
    public struct UpdateShelterUICounterCommand : IGlobalCommand
    {
        public int CounterID;
        public int Value;
        public int OldValue;
        public int MaxValue;
    }
}

namespace Components
{
}