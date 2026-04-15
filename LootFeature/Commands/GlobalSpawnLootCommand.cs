using HECSFramework.Core;

namespace Commands
{
    [Documentation(Doc.Loot, "we send this command when we need spawn loot")]
	public struct GlobalSpawnLootCommand : IGlobalCommand
	{
		public AliveEntity From;
	}

    [Documentation(Doc.Loot, "we send this command when we need spawn rewards views")]
    public struct SpawnRewardsViewsCommand : ICommand
    {
    }
}