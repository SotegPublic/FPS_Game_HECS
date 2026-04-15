using Commands;
using Components;
using HECSFramework.Core;
using System;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.UI, Doc.Shelter, "MissionsListDrawUISystem")]
    public sealed class MissionsListDrawUISystem : BaseSystem, ILateUpdatable, IReactGlobalCommand<MissionsListRedrawCommand>
    {
        [Required] public MissionsPanelStateComponent panelStateComponent;
        [Single] public RewardsGlobalHolderComponent rewardsGlobalHolderComponent;

        private CurrentMissionsHolderComponent currentMissionsHolderComponent;
        private MissionsConfigsHolderComponent missionsConfigsHolderComponent;
        private ShelterEnergyCounterComponent energyCounterComponent;
        private SurvivorsCounterComponent survivorsCounterComponent;
        private ReservedSurvivorsCounterComponent reservedSurvivorsCounter;

        public override void InitSystem()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            currentMissionsHolderComponent = player.GetComponent<CurrentMissionsHolderComponent>();
            missionsConfigsHolderComponent = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>().GetComponent<MissionsConfigsHolderComponent>();
            energyCounterComponent = player.GetComponent<ShelterEnergyCounterComponent>();
            survivorsCounterComponent = player.GetComponent<SurvivorsCounterComponent>();
            reservedSurvivorsCounter = player.GetComponent<ReservedSurvivorsCounterComponent>();
        }

        public void UpdateLateLocal()
        {
            var currentMissionsCount = currentMissionsHolderComponent.GetActiveMissionsCount();

            for (int i = 0; i < currentMissionsCount; i++)
            {
                var missionPanel = panelStateComponent.MissionPanels[i];

                if (currentMissionsHolderComponent.TryGetActiveMission(missionPanel.MissionID, out var activeMission))
                {
                    var freeSurvivors = survivorsCounterComponent.Value - reservedSurvivorsCounter.Value;
                    missionPanel.UpdatePanel(activeMission, energyCounterComponent.Value, freeSurvivors);
                }
            }
        }

        public void CommandGlobalReact(MissionsListRedrawCommand command)
        {
            ReDrawPanels();
        }

        private void ReDrawPanels()
        {
            var currentMissionsCount = currentMissionsHolderComponent.GetActiveMissionsCount();

            for (int i = 0; i < currentMissionsCount; i++)
            {
                var mission = missionsConfigsHolderComponent.GetMissionByID(currentMissionsHolderComponent.ActiveMissions[i].MissionID);
                var entermonoComponent = panelStateComponent.MissionPanels[i];

                DrawMissionPanel(mission, entermonoComponent);
            }

            HideNotUsedPanels(currentMissionsCount);
        }

        private void DrawMissionPanel(MissionConfig missionConfig, MissionPanelMonoComponent enterButtonMono)
        {
            var missionsDifficultyConfigComponent = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>().GetComponent<MissionsDifficultyConfigComponent>();
            var difficulty = missionsDifficultyConfigComponent.GetDifficultyValue(missionConfig.MissionGrade, missionConfig.MissionType, missionConfig.StartZone);

            enterButtonMono.InitPanel(missionConfig, difficulty, rewardsGlobalHolderComponent);
            enterButtonMono.ChangePanelVisibility(true);
        }

        private void HideNotUsedPanels(int currentMissionsCount)
        {
            for (int i = currentMissionsCount; i < panelStateComponent.MissionPanels.Count; i++)
            {
                panelStateComponent.MissionPanels[i].ChangePanelVisibility(false);
            }
        }
    }
}
