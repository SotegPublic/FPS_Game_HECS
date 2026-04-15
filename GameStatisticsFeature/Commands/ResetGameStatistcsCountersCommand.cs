using System;
using HECSFramework.Core;

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Statistics, "ResetGameStatistcsCounters")]
    public struct ResetGameStatistcsCountersCommand : IGlobalCommand
    {
    }
}