using Commands;
using Components;
using HECSFramework.Core;
using System;
using TMPro;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.UI, Doc.Shelter, "SelectEnterPanelUISystem")]
    public sealed class MissionsPanelUISystem : BaseSystem, IAfterEntityInit, IReactCommand<ShowShelterPanelCommand>, ILateUpdatable
    {
        [Required] private UIAccessPrfbProviderComponent accessPrfbProvider;
        [Required] private UIAccessProviderComponent accessProvider;
        [Required] public MissionsPanelStateComponent panelStateComponent;

        private ShelterEnergyCounterComponent energyCounterComponent;
        private SurvivorsCounterComponent survivorsCounterComponent;
        private ReservedSurvivorsCounterComponent reservedSurvivorsCounterComponent;
        private CurrentMissionsHolderComponent currentMissionsHolderComponent;
        private MissionsConfigsHolderComponent missionsConfigsHolderComponent;
        private TMP_Text timerText;
        private bool isSystemActive;

        private const int PANEL_ID = UIAccessIdentifierMap.Missions;

        public override void InitSystem()
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            currentMissionsHolderComponent = player.GetComponent<CurrentMissionsHolderComponent>();
            missionsConfigsHolderComponent = Owner.World.GetEntityBySingleComponent<RaidManagerTagComponent>().GetComponent<MissionsConfigsHolderComponent>();
            energyCounterComponent = player.GetComponent<ShelterEnergyCounterComponent>();
            survivorsCounterComponent = player.GetComponent<SurvivorsCounterComponent>();
            reservedSurvivorsCounterComponent = player.GetComponent<ReservedSurvivorsCounterComponent>();

            panelStateComponent.MissionPanels = new HECSList<MissionPanelMonoComponent>(currentMissionsHolderComponent.MissionsListLength);
        }

        public void AfterEntityInit()
        {
            InitMainMissionsPanel();
            Owner.World.Command(new MissionsListRedrawCommand());
            isSystemActive = true;
        }

        public void UpdateLateLocal()
        {
            if (!isSystemActive)
                return;

            var updateTime = currentMissionsHolderComponent.LastMissionsUpdateTime + currentMissionsHolderComponent.UpdateListFrequencyInSec;
            var timeRemainForUpdate = updateTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            timerText.text = TimeSpan.FromSeconds(timeRemainForUpdate).ToString(@"hh\:mm\:ss");
        }

        private void InitMainMissionsPanel()
        {
            var panelUIAccess = accessProvider.Get.GetUIAccess(PANEL_ID);
            CreateMissionPanels(panelUIAccess);

            var closeNotEnoughPanel = panelUIAccess.GetButton(UIAccessIdentifierMap.Exit);
            closeNotEnoughPanel.onClick.AddListener(CloseNotEnoughWindow);

            var forceUpdateMissionsBtn = panelUIAccess.GetButton(UIAccessIdentifierMap.Reset);
            forceUpdateMissionsBtn.onClick.AddListener(UpdateMissionsList);

            timerText = panelUIAccess.GetTextMeshProUGUI(UIAccessIdentifierMap.Timer);
        }

        private void UpdateMissionsList()
        {
            Owner.World.Command(new UpdateMissionsListCommand());
        }

        private void CreateMissionPanels(UIAccessMonoComponent panelUIAccess)
        {
            var root = panelUIAccess.GetRectTransform(UIAccessIdentifierMap.Root);
            var prfb = accessPrfbProvider.Get.GetPrefab(UIAccessIdentifierMap.SelectZoneButton);

            for (int i = 0; i < currentMissionsHolderComponent.MissionsListLength; i++)
            {
                var entermonoComponent = MonoBehaviour.Instantiate(prfb, root).GetComponent<MissionPanelMonoComponent>();
                entermonoComponent.OnStartMissionClick += StartMissionReact;
                entermonoComponent.OnFinishMissionClick += FinishMissionReact;
                panelStateComponent.MissionPanels.Add(entermonoComponent);
                entermonoComponent.ChangePanelVisibility(false);
            }
        }

        public void CommandReact(ShowShelterPanelCommand command)
        {
            if (command.PanelID != PANEL_ID)
                return;

            // for dynamic UI elements
        }

        private void FinishMissionReact(MissionPanelMonoComponent missionPanel)
        {
            Owner.World.Command(new CompleteMissionCommand { MissionID = missionPanel.MissionID });
        }

        private void StartMissionReact(MissionPanelMonoComponent missionPanel, bool isSelfMission)
        {
            var mission = missionsConfigsHolderComponent.GetMissionByID(missionPanel.MissionID);

            if (isSelfMission)
            {
                TryGoOnMission(mission.StartZone, mission.MissionId, mission.MissionCost);
            }
            else
            {
                TrySendSurvivors(missionPanel, mission.ConfigForSurvivors.RequiredSurvivors);
            }
        }

        private void TryGoOnMission(int enterID, int missionID, int cost)
        {
            if (energyCounterComponent.Value < cost)
            {
                ShowNotEnoughWindow(MissionStartResourcesType.Energy);
            }
            else
            {
                Owner.World.Command(new SelectMissionCommand { EnterZoneID = enterID, MissionID = missionID, Cost = cost });
            }
        }

        private void TrySendSurvivors(MissionPanelMonoComponent missionPanel, int survivorsNeeded)
        {
            var freeSurvivors = survivorsCounterComponent.Value - reservedSurvivorsCounterComponent.Value;
            if (freeSurvivors < survivorsNeeded)
            {
                ShowNotEnoughWindow(MissionStartResourcesType.Survivors);
            }
            else
            {
                Owner.World.Command(new StartSurvivorsMissionCommand { MissionID = missionPanel.MissionID});
            }
        }

        private void ShowNotEnoughWindow(MissionStartResourcesType startResourcesType)
        {
            var panelUIAccess = accessProvider.Get.GetUIAccess(PANEL_ID);
            var canvasGroup = panelUIAccess.GetCanvasGroup(UIAccessIdentifierMap.CanvasGroup);

            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            var startText = "Not enough ";
            var resourceText = startResourcesType switch
            {
                MissionStartResourcesType.Energy => "energy ",
                MissionStartResourcesType.Survivors => "survivors ",
                _ => "resources "
            };
            var endText = "to launch the mission";

            var panelText = panelUIAccess.GetTextMeshProUGUI(UIAccessIdentifierMap.Text);
            panelText.text = startText + resourceText + endText;
        }

        private void CloseNotEnoughWindow()
        {
            var panelUIAccess = accessProvider.Get.GetUIAccess(PANEL_ID);
            var canvasGroup = panelUIAccess.GetCanvasGroup(UIAccessIdentifierMap.CanvasGroup);

            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            isSystemActive = false;
            DestroyPanels();
            panelStateComponent.Clear();

            var panelUIAccess = accessProvider.Get.GetUIAccess(PANEL_ID);
            var closeNotEnoughPanel = panelUIAccess.GetButton(UIAccessIdentifierMap.Exit);
            closeNotEnoughPanel.onClick.RemoveListener(CloseNotEnoughWindow);

            var forceUpdateMissionsBtn = panelUIAccess.GetButton(UIAccessIdentifierMap.Reset);
            forceUpdateMissionsBtn.onClick.RemoveListener(UpdateMissionsList);
        }

        private void DestroyPanels()
        {
            for (int i = 0; i < panelStateComponent.MissionPanels.Count; i++)
            {
                panelStateComponent.MissionPanels[i].OnStartMissionClick -= StartMissionReact;
                panelStateComponent.MissionPanels[i].OnFinishMissionClick -= FinishMissionReact;
                GameObject.Destroy(panelStateComponent.MissionPanels[i]);
            }
        }
    }

    public enum MissionStartResourcesType
    {
        None = 0,
        Energy = 1,
        Survivors = 2
    }
}