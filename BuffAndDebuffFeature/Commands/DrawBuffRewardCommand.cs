using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.DrawRule, Doc.Visual, Doc.Item, "this command from global reward system to draw entities for executing visual scenarios of buff rewards")]
    public struct DrawBuffRewardCommand : ICommand
    {
        public GlobalBuffRewardCommand GlobalBuffRewardCommand;
    }
}