using System;
using HECSFramework.Core;
using HECSFramework.Unity;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.ShootingZoneFeature, "this component provides access to points for spawn resources")]
    public sealed class ResourceSpawnPointsZoneHolderComponent : BaseComponent, IHaveActor
    {
        public Actor Actor { get; set; }

        public ResourceSpawnPointMonoComponent[] Points;

        public override void Init()
        {
            Actor.TryGetComponents(out Points);
        }
    }
}