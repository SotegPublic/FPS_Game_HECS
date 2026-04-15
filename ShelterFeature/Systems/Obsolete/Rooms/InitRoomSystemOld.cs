using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;

namespace Systems
{
    [Obsolete]
	[Serializable][Documentation(Doc.Shelter, "this system init room")]
    public sealed class InitRoomSystemOld : BaseSystem 
    {
        [Required] private CountersHolderComponent countersHolder;
        [Required] private RoomServiceConfigComponent serviceConfig;

        public override void InitSystem()
        {
            for(int i = 0; i < serviceConfig.ResourcesForService.Length; i++)
            {
                countersHolder.GetOrAddIntCounter(serviceConfig.ResourcesForService[i].ResourceID);
            }
        }
    }
}