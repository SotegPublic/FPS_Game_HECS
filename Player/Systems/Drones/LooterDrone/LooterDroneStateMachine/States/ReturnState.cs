using Components;
using HECSFramework.Core;

public class ReturnState : LooterDroneState
{
    private bool isActive;

    public ReturnState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
    }

    public override int StateID => ReturnState;

    public override void Enter(Entity entity)
    {
        var offset = entity.GetComponent<DroneFollowConfigComponent>().StayPositionOffset;
        var target = entity.GetComponent<DroneFollowTargetComponent>().FollowTarget;
        var targetPosition = target.transform.TransformPoint(offset);

        var tag = entity.GetOrAddComponent<MoveByCurveToV3TagComponent>();
        tag.From = entity.GetPosition();
        tag.To = targetPosition;
        tag.DrawRule = MoveByCurveDrawRuleIdentifierMap.LooterDroneCurveIdentifier;

        isActive = true;
    }

    public override void Exit(Entity entity)
    {
    }

    public override void Update(Entity entity)
    {
        if (!isActive)
            return;
        if (entity.ContainsMask<MoveByCurveToV3TagComponent>())
            return;

        isActive = false;
        EndState();
    }
}
