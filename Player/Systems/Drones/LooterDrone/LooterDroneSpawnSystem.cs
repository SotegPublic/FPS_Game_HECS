using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Systems
{
	[Serializable][Documentation(Doc.Drone, Doc.Spawn, "this system spawn looter drone")]
    public sealed class LooterDroneSpawnSystem : BaseSystem, IInitAfterView
    {
        [Required] private LooterDroneContainersHolderComponent containersHolder;
        [Required] private DroneSpawnPointProviderComponent spawnPointsProvider;

        public void InitAfterView()
        {
            SpawnDroneAsync().Forget();
        }

        private async UniTask SpawnDroneAsync()
        {
            if (containersHolder.TryGetContainerByID(EntityContainersMap._DefaultLooterDroneContainer, out var droneContainer))
            {
                if (spawnPointsProvider.TryGetFreeSpawnPoint(out var spawnPoint))
                {
                    var droneActor = await droneContainer.GetActor(position: spawnPoint.Transform.position);
                    droneActor.Init();

                    droneActor.Entity.GetOrAddComponent<DroneFollowTargetComponent>().FollowTarget = spawnPoint.Transform;
                }
            }
        }

        public async override void InitSystem()
        {
        }

        public void Reset()
        {
        }
    }
}