using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable][Documentation(Doc.Lair, Doc.Holder, "here we hold lair cooldowns info")]
    public sealed class GlobalLairsCooldownsHolderComponent : BaseComponent, IWorldSingleComponent
    {
        private HECSList<LairCooldown> lairsCooldowns = new HECSList<LairCooldown>(32);
        private HashSet<int> lairsID = new HashSet<int>(32);

        public ReadonlyHECSList<LairCooldown> LairsCooldowns;

        public override void Init()
        {
            LairsCooldowns = new ReadonlyHECSList<LairCooldown>(lairsCooldowns);
        }

        public void AddLairCooldown(int zoneID, float cooldown)
        {
            lairsCooldowns.Add(new LairCooldown { ZoneID = zoneID, Cooldown = cooldown });
            lairsID.Add(zoneID);
        }

        public void UpdateAllCooldownsAndRemoveExpired(float delta)
        {
            for (int i = lairsCooldowns.Count - 1; i >= 0; i--)
            {
                ref var lairCooldown = ref lairsCooldowns[i];
                lairCooldown.Cooldown -= delta;

                if (lairCooldown.Cooldown <= 0)
                {
                    lairsID.Remove(lairCooldown.ZoneID);
                    lairsCooldowns.RemoveAtSwap(i);
                }
            }
        }

        public bool ContainsZoneID(int zoneID)
        {
            return lairsID.Contains(zoneID);
        }

        public void ResetAllCooldowns()
        {
            lairsID.Clear();
            lairsCooldowns.ClearFast();
        }
    }

    public struct LairCooldown
    {
        public int ZoneID;
        public float Cooldown;
    }
}