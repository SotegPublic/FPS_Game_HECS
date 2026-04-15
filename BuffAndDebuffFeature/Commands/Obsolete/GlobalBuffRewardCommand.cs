using HECSFramework.Core;
using System;
using UnityEngine;

namespace Commands
{
    [Obsolete]
    [Documentation(Doc.Visual, Doc.Rewards, Doc.UI, "GlobalBuffRewardCommand")]
    public struct GlobalBuffRewardCommand : IGlobalCommand
    {
        public AliveEntity From;
        public AliveEntity To;
        public GameObject RewardView;
        public int BuffID;
        public int DrawRule;
    }
}