using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Shelter, "ShelterModeInProgressStateSystem")]
    public sealed class ShelterModeInProgressStateSystem : BaseGameStateSystem, IReactGlobalCommand<SelectMissionCommand>
    {
        protected override int State => GameStateIdentifierMap.ShelterModeInProgress;

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {
            Owner.AddComponent<ShelterSceneActiveTagComponent>();
        }

        protected override void OnExitState()
        {
            base.OnExitState();
            Owner.RemoveComponent<ShelterSceneActiveTagComponent>();
        }

        public void CommandGlobalReact(SelectMissionCommand command)
        {
            var shooterZoneStateComponent = Owner.World.GetSingleComponent<ShooterZoneStateComponent>();
            shooterZoneStateComponent.EnterID = command.EnterZoneID;
            shooterZoneStateComponent.MissionID = command.MissionID;

            EndState();
        }
    }
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Shelter, "SelectMissionCommand")]
    public struct SelectMissionCommand : IGlobalCommand
    {
        public int EnterZoneID;
        public int MissionID;
        public int Cost;
    }
}