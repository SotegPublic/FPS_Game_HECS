using System;
using HECSFramework.Core;

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Statistics, "UpdateGameStatistcsFloatCounter")]
    public struct UpdateGameStatistcsFloatCounterCommand : IGlobalCommand
    {
        public int CounterID;
        public float Value;
    }
}