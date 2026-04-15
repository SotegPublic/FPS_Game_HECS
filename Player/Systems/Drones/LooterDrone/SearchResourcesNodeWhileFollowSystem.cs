using System;
using HECSFramework.Core;
using UnityEngine;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Resources, Doc.Drone, "this system searches resources nodes when drone follow player")]
    public sealed class SearchResourcesNodeWhileFollowSystem : BaseSystem, IUpdatable
    {
        [Required] private DroneCollectWhileFollowConfigComponent config;

        private EntitiesFilter filter;

        public override void InitSystem()
        {
            filter = Owner.World.GetFilter(Filter.Get<ResourcesNodeTagComponent>(), Filter.Get<IsDeadTagComponent>());
        }

        public void UpdateLocal()
        {
            if (!Owner.ContainsMask<DroneFollowTagComponent>())
                return;

            if (Owner.World.TryGetSingleComponent(out PlayerCharacterComponent playerCharacterComponent))
            {
                var playerCharacter = playerCharacterComponent.Owner;

                foreach (var node in filter)
                {
                    if (node.ContainsMask<NodeCollectedTagComponent>())
                        continue;

                    var nodePosition = node.GetPosition();
                    var sqrDistance = (nodePosition - playerCharacter.GetPosition()).sqrMagnitude;

                    if (sqrDistance < config.SqrSearchDistance)
                    {
                        GetCollectPoint(node);
                        break;
                    }
                }
            }
        }

        private void GetCollectPoint(Entity node)
        {
            var character = Owner.World.GetEntityBySingleComponent<PlayerCharacterComponent>();
            var collectWhileFollowConfig = Owner.GetComponent<DroneCollectWhileFollowConfigComponent>();

            var targetPosition = node.GetPosition();
            var characterPosition = character.GetPosition();
            var characterForward = character.GetTransform().forward;

            var projectedResourcePoint = characterPosition + Vector3.Project(
                targetPosition - characterPosition,
                characterForward
            );

            projectedResourcePoint.y = targetPosition.y + collectWhileFollowConfig.HeightOffset;

            Owner.GetOrAddComponent<CollectPointComponent>().Point = projectedResourcePoint;
        }
    }
}