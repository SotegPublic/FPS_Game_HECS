using Components;
using HECSFramework.Core;
using TMPro;
using UnityEngine;

public class GoToLootState : LooterDroneState
{
    private bool isActive;

    public GoToLootState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
    }

    public override int StateID => GoToLootState;

    public override void Enter(Entity entity)
    {
        var config = entity.GetComponent<DroneLootCollectingConfigComponent>();

        var targetPoint  = entity.GetComponent<CurrentLootTargetComponent>().CurrentCollectLootPosition + Vector3.up * config.HeightOffset;
        var direction = targetPoint - entity.GetPosition();

        if (direction.sqrMagnitude > config.SqrCollectDistanceOffset)
        {
            var normalizedDirection = direction.normalized;
            targetPoint = targetPoint - normalizedDirection * config.CollectDistanceOffset;

            var tag = entity.GetOrAddComponent<MoveByCurveToV3TagComponent>();
            tag.From = entity.GetPosition();
            tag.To = targetPoint;
            tag.DrawRule = MoveByCurveDrawRuleIdentifierMap.LooterDroneCurveIdentifier;

            isActive = true;
        }
        else
        {
            EndState();
        }
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

public class EndMoveByCurveTransition : ITransition
{
    private Entity owner;
    public EndMoveByCurveTransition(Entity owner, int toState)
    {
        ToState = toState;
        this.owner = owner;
    }

    public int ToState { get; }

    public bool IsReady()
    {
        return !owner.ContainsMask<MoveByCurveToV3TagComponent>();
    }
}