using Components;
using HECSFramework.Core;


namespace Strategies
{
    [Documentation(Doc.Strategy, "MoveForwardNode")]
    public class MoveForwardNode : InterDecision
    {
        public override string TitleOfNode { get; } = "MoveForwardNode";

        [Connection(ConnectionPointType.In, "<float> Delta time")]
        public GenericNode<float> DeltaTime;

        protected override void Run(Entity entity)
        {
            var room = entity.GetComponent<SurvivorRoomHolderComponent>().Room;
            var roomTransform = room.GetTransform();

            var moveSpeed = entity.GetComponent<MoveSpeedComponent>().WalkingSpeed;
            var survivorTransform = entity.GetTransform();
            var newPosition = roomTransform.InverseTransformPoint(survivorTransform.position);
            var directionModifier = entity.GetComponent<OnMoveTagComponent>().Direction == SurvivorDirection.Right ? 1 : -1;

            newPosition.x += (moveSpeed * directionModifier) * DeltaTime.Value(entity);

            survivorTransform.position = roomTransform.TransformPoint(newPosition);

            Next.Execute(entity);
        }
    }
}
