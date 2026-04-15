using System;
using HECSFramework.Core;

namespace Commands
{
    [Serializable][Documentation(Doc.Statistics, "UpdateGameStatistcsIntCounter")]
    public struct UpdateGameStatistcsIntCounterCommand : IGlobalCommand
    {
        public int CounterID;
        public int Value;
    }
}