using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Resources, Doc.Spawn, "this system spawn nodes in resources missions")]
    public sealed class SpawnResourceNodesSystem : BaseSystem, IReactCommand<SpawnResourceNodesCommand>
    {
        [Required] public AssetRefIDHolderComponent AssetRefsHolder;
        [Required] public ResourceSpawnPointsZoneHolderComponent ResourceSpawnPoints;

        [Single] public PoolingSystem PoolingSystem;

        public void CommandReact(SpawnResourceNodesCommand command)
        {
            var refID = Owner.GetComponent<ResourcesZoneTagComponent>().ResourceID;
            var assetRef = AssetRefsHolder.GetRef(refID);

            for (int i = 0; i < ResourceSpawnPoints.Points.Length; i++)
            {
                var point = ResourceSpawnPoints.Points[i];
                PoolingSystem.GetViewFromPool(assetRef, position: point.transform.position).Forget();
            }
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Resources, Doc.Spawn, "we send this command for spawn nodes with resources")]
    public struct SpawnResourceNodesCommand : ICommand { }
}