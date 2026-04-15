using Components;
using HECSFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Strategies
{
    [Documentation(Doc.Strategy, "GetTargetNode")]
    public class GetTargetNode : InterDecision
    {
        public override string TitleOfNode { get; } = "GetTargetNode";

        protected override void Run(Entity entity)
        {
            var room = entity.GetComponent<SurvivorRoomHolderComponent>().Room;
            var roomTransform = room.GetTransform();
            var roomMonocomponent = room.GetComponent<RoomMonocomponentProviderComponent>().Get;
            var survivorTransform = entity.GetTransform();

            var localLeftEdge = roomTransform.InverseTransformPoint(roomMonocomponent.LeftEdge.transform.position);
            var localRightEdge = roomTransform.InverseTransformPoint(roomMonocomponent.RightEdge.transform.position);
            var localFloorPosition = roomTransform.InverseTransformPoint(roomMonocomponent.FloorEdge.transform.position);
            var localSurvivorPosition = roomTransform.InverseTransformPoint(survivorTransform.position);

            var newX = Random.Range(localLeftEdge.x, localRightEdge.x);

            var moveTag = entity.AddComponent<OnMoveTagComponent>();
            moveTag.TargetLocalPositionInRoom = new Vector3(newX, localFloorPosition.y, localSurvivorPosition.z);

            var direction = localSurvivorPosition.x > newX ? SurvivorDirection.Left : SurvivorDirection.Right;
            moveTag.Direction = direction;

            var newRotationTarget = direction == SurvivorDirection.Right ? Quaternion.LookRotation(Vector3.right) : Quaternion.LookRotation(Vector3.left);
            survivorTransform.rotation = newRotationTarget;

            Next.Execute(entity);
        }
    }
}
