using System;
using HECSFramework.Core;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Raid, Doc.Path, "this system create path tree after raid level was loaded")]
    public sealed class CreateRaidPathTreeSystem : BaseSystem, IReactGlobalCommand<AfterCommand<MissionZoneLoadedCommand>>
    {
        [Required] private RaidPathTreeHolderComponent treeHolder;

        private EntitiesFilter zonesFilter;

        public void CommandGlobalReact(AfterCommand<MissionZoneLoadedCommand> command)
        {
            zonesFilter.ForceUpdateFilter();
            treeHolder.ClearPathTree();

            foreach(var zone in zonesFilter)
            {
                var avalibleZones = zone.GetComponent<ZoneLinksComponent>().LinkedZones;
                var index = zone.GetComponent<ZoneIndexComponent>().Index;
                var bonuses = new HECSList<int>(8);

                if (zone.ContainsMask<ResourcesZoneTagComponent>())
                    bonuses.Add(ZoneBonusIdentifierMap.Resources);

                if (zone.ContainsMask<EnterZoneTagComponent>())
                    bonuses.Add(ZoneBonusIdentifierMap.Enter);

                var isLair = false;

                if (zone.TryGetComponent<LairConfigComponent>(out var config))
                {
                    isLair = true;
                    var lairBonus = config.ChestSpawnChance switch
                    {
                        0 => ZoneBonusIdentifierMap.Boss,
                        1 => ZoneBonusIdentifierMap.Chest,
                        _ => ZoneBonusIdentifierMap.RandomLair
                    };

                    bonuses.Add(lairBonus);
                }

                treeHolder.AddNode(new PathNode
                {
                    ZoneID = index,
                    IsLair = isLair,
                    Bonuses = bonuses,
                    AvailablePathIDs = avalibleZones
                });
            }
        }

        public override void InitSystem()
        {
            zonesFilter = Owner.World.GetFilter<ZoneTagComponent>();
        }
    }
}