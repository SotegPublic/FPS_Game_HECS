using System;
using HECSFramework.Core;
using Components;
using Commands;
using HECSFramework.Unity;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Loot, "this system disassembly all loot in drone inventory into yellow crystals")]
    public sealed class LooterDroneDisassemblySystem : BaseSystem, IGlobalStart, IReactGlobalCommand<LootWindowClosedCommand>
    {
        [Required] private DroneInventoryComponent droneInventory;

        private LootItemsByTypeDisassemblyConfigComponent disassemblyConfig;
        private GradesDisassemblyModifiersConfigComponent gradeModifiersConfigs;

        public void CommandGlobalReact(LootWindowClosedCommand command)
        {
            using var items = droneInventory.GetItems();

            for (int i = 0; i < items.Count; i++) 
            {
                var value = items.Items[i].Value;

                if (value.IsAvailable)
                    continue;

                DisasseblyItemInInventory(value);
                droneInventory.RemoveItem(i);
            }
        }

        private void DisasseblyItemInInventory(InventoryItem item)
        {
            var disassemblyResources = disassemblyConfig.GetDisassemblyResources(item.ItemType);
            var gradeModifier = gradeModifiersConfigs.GetModifier(item.Grade);
            var raidCountersOwner = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>();
            var raidCountersHolder = raidCountersOwner.GetComponent<CountersHolderComponent>();

            foreach(var resource in disassemblyResources)
            {
                var counter = raidCountersHolder.GetOrAddIntCounter(resource.ResourceID);
                var value = (int)(resource.Value * gradeModifier) * item.Count;

                counter.ChangeValue(value);

                Owner.World.Command(new UpdateVisualRewardCounterCommand
                {
                    CounterID = resource.ResourceID,
                    Amount = value,
                    To = raidCountersOwner
                });
            }
        }

        public void GlobalStart()
        {
            disassemblyConfig = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<LootItemsByTypeDisassemblyConfigComponent>();
            gradeModifiersConfigs = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<GradesDisassemblyModifiersConfigComponent>();
        }

        public override void InitSystem()
        {
        }
    }
}