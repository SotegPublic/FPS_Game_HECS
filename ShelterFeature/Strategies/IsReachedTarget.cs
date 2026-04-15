using Components;
using HECSFramework.Core;
using UnityEngine;


namespace Strategies
{
    [Documentation(Doc.Strategy, "IsReachedTarget")]
    public class IsReachedTarget : DilemmaDecision
    {
        public override string TitleOfNode { get; } = "IsReachedTarget";

        protected override void Run(Entity entity)
        {
            var room = entity.GetComponent<SurvivorRoomHolderComponent>().Room;
            var roomTransform = room.GetTransform();

            var tag = entity.GetComponent<OnMoveTagComponent>();
            var target = tag.TargetLocalPositionInRoom;

            var currentLocalPos = roomTransform.InverseTransformPoint(entity.GetPosition());
            var direction = tag.Direction;

            var isReached = direction switch
            {
                SurvivorDirection.Left => currentLocalPos.x <= target.x,
                SurvivorDirection.Right => currentLocalPos.x >= target.x,
                SurvivorDirection.None or _ => false
            };

            if (isReached)
            {
                Positive.Execute(entity);
                return;
            }
            else
            {
                Negative.Execute(entity);
                return;
            }
        }
    }
}
