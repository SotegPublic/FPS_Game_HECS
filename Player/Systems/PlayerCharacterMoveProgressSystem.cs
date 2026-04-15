using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Character, "this system calculate remainig distance and progress between shooting zones")]
    public sealed class PlayerCharacterMoveProgressSystem : BaseSystem, IUpdatable,
        IRequestProvider<CharacterMoveProgressRequestResult, CharacterMoveProgressRequestCommand>,
        IRequestProvider<CharacterLineMoveProgressRequestResult, CharacterLineMoveProgressRequestCommand>,
        IReactGlobalCommand<StartZoneCommand>
    {
        [Required] private NavMeshAgentComponent agentComponent;

        private ShooterZoneStateComponent zoneState;
        private float fullDistanceToNextShootingPoint;
        private float lineDistanceToNextShootingPoint;
        private float progress;

        public override void InitSystem()
        {
            zoneState = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
        }

        public void CommandGlobalReact(StartZoneCommand command)
        {
            fullDistanceToNextShootingPoint = 0;
            progress = 0;
            lineDistanceToNextShootingPoint = 0;
        }

        public CharacterMoveProgressRequestResult Request(CharacterMoveProgressRequestCommand command)
        {
            var isNoPathOrPathPending = !agentComponent.NavMeshAgent.hasPath || (agentComponent.NavMeshAgent.hasPath && agentComponent.NavMeshAgent.pathPending);

            if (Owner.ContainsMask<OnStopMovingComponent>() || isNoPathOrPathPending)
            {
                return new CharacterMoveProgressRequestResult
                {
                    IsNoPath = true,
                    Progress = 0f,
                    RemainingDistance = 0f,
                    StopingDistance = 0f
                };
            }

            return new CharacterMoveProgressRequestResult
            {
                IsNoPath = false,
                Progress = progress,
                RemainingDistance = agentComponent.NavMeshAgent.remainingDistance,
                StopingDistance = agentComponent.NavMeshAgent.stoppingDistance
            };
        }

        public CharacterLineMoveProgressRequestResult Request(CharacterLineMoveProgressRequestCommand command)
        {
            var currentLineDirection = zoneState.CurrentShootingPoint.Entity.GetPosition() - Owner.GetPosition();
            currentLineDirection.y = 0;
            var currentLineDistance = currentLineDirection.magnitude;
            var isLessThenStopping = currentLineDistance < agentComponent.NavMeshAgent.stoppingDistance;

            return new CharacterLineMoveProgressRequestResult
            {
                StartDistance = lineDistanceToNextShootingPoint,
                CurrentDistance = currentLineDistance,
                IsLessThenStoppingDistance = isLessThenStopping
            };
        }

        public void UpdateLocal()
        {
            if (Owner.ContainsMask<OnStopMovingComponent>())
                return;

            if (!agentComponent.NavMeshAgent.hasPath || (agentComponent.NavMeshAgent.hasPath && agentComponent.NavMeshAgent.pathPending))
                return;

            if (lineDistanceToNextShootingPoint == 0)
                lineDistanceToNextShootingPoint = (zoneState.CurrentShootingPoint.Entity.GetPosition() - Owner.GetPosition()).magnitude;

            if (fullDistanceToNextShootingPoint == 0 || float.IsInfinity(fullDistanceToNextShootingPoint))
                fullDistanceToNextShootingPoint = agentComponent.NavMeshAgent.remainingDistance;

            progress = 1 - (agentComponent.NavMeshAgent.remainingDistance / fullDistanceToNextShootingPoint);
        }
    }


    public struct CharacterMoveProgressRequestResult
    {
        public bool IsNoPath;
        public float Progress;
        public float RemainingDistance;
        public float StopingDistance;
    }

    public struct CharacterLineMoveProgressRequestResult
    {
        public bool IsLessThenStoppingDistance;
        public float CurrentDistance;
        public float StartDistance;
    }
}

namespace Commands
{
    [Documentation(Doc.Movement, "by this command we request move progress on full path")]
    public struct CharacterMoveProgressRequestCommand : ICommand
    {
    }

    [Documentation(Doc.Movement, "by this command we request straight line move progress")]
    public struct CharacterLineMoveProgressRequestCommand : ICommand
    {
    }
}