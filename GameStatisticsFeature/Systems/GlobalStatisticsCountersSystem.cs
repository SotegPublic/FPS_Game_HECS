using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Statistics, "this system manage game statistics counters")]
    public sealed class GlobalStatisticsCountersSystem : BaseSystem, IReactGlobalCommand<UpdateGameStatistcsIntCounterCommand>,
        IReactGlobalCommand<UpdateGameStatistcsFloatCounterCommand>,
        IReactGlobalCommand<CleanRaidGlobalCommand>, IRequestProvider<GetFloatStatisticsCounterRequestResult, GetFloatStatisticsCounterRequestCommand>,
        IRequestProvider<GetIntStatisticsCounterRequestResult, GetIntStatisticsCounterRequestCommand>
    {
        [Required] private CountersHolderComponent countersHolder;

        public override void InitSystem()
        {
        }

        public void CommandGlobalReact(UpdateGameStatistcsIntCounterCommand command)
        {
            var counter = countersHolder.GetOrAddIntCounter(command.CounterID);
            counter.ChangeValue(command.Value);
        }

        public void CommandGlobalReact(UpdateGameStatistcsFloatCounterCommand command)
        {
            var counter = countersHolder.GetOrAddFloatCounter(command.CounterID);
            counter.ChangeValue(command.Value);
        }

        public void CommandGlobalReact(CleanRaidGlobalCommand command)
        {
            countersHolder.ResetCountersToZero();
        }

        public GetFloatStatisticsCounterRequestResult Request(GetFloatStatisticsCounterRequestCommand command)
        {
            var counter = countersHolder.GetOrAddFloatCounter(command.CounterID);
            return new GetFloatStatisticsCounterRequestResult { Value = counter.Value };
        }

        public GetIntStatisticsCounterRequestResult Request(GetIntStatisticsCounterRequestCommand command)
        {
            var counter = countersHolder.GetOrAddIntCounter(command.CounterID);
            return new GetIntStatisticsCounterRequestResult { Value = counter.Value };
        }
    }
}

public struct GetFloatStatisticsCounterRequestResult
{
    public float Value;
}

public struct GetIntStatisticsCounterRequestResult
{
    public int Value;
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Statistics, "GetFloatStatisticsCounterRequestCommand")]
    public struct GetFloatStatisticsCounterRequestCommand : ICommand
    {
        public int CounterID;
    }

    [Serializable]
    [Documentation(Doc.Statistics, "GetFloatStatisticsCounterRequestCommand")]
    public struct GetIntStatisticsCounterRequestCommand : ICommand
    {
        public int CounterID;
    }
}