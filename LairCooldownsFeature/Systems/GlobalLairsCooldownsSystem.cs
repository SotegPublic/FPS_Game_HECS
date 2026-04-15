using System;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Lair, Doc.Raid, "this system get and update lairs cooldowns in cooldowns holder component")]
    public sealed class GlobalLairsCooldownsSystem : BaseSystem, IGlobalStart, ILateUpdatable, IReactGlobalCommand<LairZoneCompleteCommand>
    {
        [Required] private GlobalLairsCooldownsHolderComponent cooldownsHolder;

        public void CommandGlobalReact(LairZoneCompleteCommand command)
        {
            var cooldown = command.LairZone.GetComponent<LairConfigComponent>().Cooldown;
            var zoneID = command.LairZone.GetComponent<ZoneIndexComponent>().Index;
            
            cooldownsHolder.AddLairCooldown(zoneID, cooldown);
        }

        public void GlobalStart()
        {
            var exitTime = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<PlayerExitTimeComponent>().ExitTime;

            if (exitTime == 0)
                return;

            var delta = GetCurrentUnixTimestamp() - exitTime;
            cooldownsHolder.UpdateAllCooldownsAndRemoveExpired(delta);

        }

        public override void InitSystem()
        {
        }

        public void UpdateLateLocal()
        {
            var delta = Time.deltaTime;
            cooldownsHolder.UpdateAllCooldownsAndRemoveExpired(delta);
        }

        private long GetCurrentUnixTimestamp() //Unix Timestamp
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.UtcNow - origin;
            return (long)diff.TotalSeconds;
        }
    }
}