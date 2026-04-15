using Components;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanelMonoComponent : MonoBehaviour
{
    public Action<MissionPanelMonoComponent, bool> OnStartMissionClick;
    public Action<MissionPanelMonoComponent> OnFinishMissionClick;

    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private Button startMissionButton;
    [SerializeField] private Button sendSurvivorsButton;
    [SerializeField] private Button getRewardButton;
    [SerializeField] private TMP_Text missionRewardText;
    [SerializeField] private TMP_Text missionTypeText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text regionText;
    [SerializeField] private TMP_Text startCostText;
    [SerializeField] private TMP_Text requiredSurvivorsText;
    [SerializeField] private Image missionImage;
    [SerializeField] private TMP_Text timerText;

    [SerializeField] private CanvasGroup awaitCanvasGroup;
    [SerializeField] private CanvasGroup inProgressCanvasGroup;
    [SerializeField] private CanvasGroup finishedCanvasGroup;

    private MissionPanelState currentState;
    private int missionID;
    private int cost;
    private int survivorsNeeded;

    public int MissionID => missionID;

    private void Awake()
    {
        startMissionButton.onClick.AddListener(StartMissionClick);
        sendSurvivorsButton.onClick.AddListener(SendOnMissionClick);
        getRewardButton.onClick.AddListener(GetMissionRewardClick);
        ChangeState(MissionPanelState.Await);
    }

    public void InitPanel(MissionConfig missionConfig, int diffuciltyID, RewardsGlobalHolderComponent rewardsHolder)
    {
        var rewardText = "";

        if (rewardsHolder.TryGetContainerByID(missionConfig.RewardID, out var container))
        {
            rewardText = container.GetComponent<NameComponent>().DefaultName;
        }

        missionRewardText.text = "Reward: " + rewardText;
        missionTypeText.text = "Mission Type: " + IdentifierToStringMap.IntToString[missionConfig.MissionType];
        difficultyText.text = "Difficulty: " + diffuciltyID.ToString();
        regionText.text = "Region: " + IdentifierToStringMap.IntToString[missionConfig.StartZone];
        missionImage.sprite = missionConfig.MissionImage;
        startCostText.text = missionConfig.MissionCost.ToString();

        if (missionConfig.IsScenarioMission)
            sendSurvivorsButton.gameObject.SetActive(false);
        else
            requiredSurvivorsText.text = missionConfig.ConfigForSurvivors.RequiredSurvivors.ToString();

        missionID = missionConfig.MissionId;
        cost = missionConfig.MissionCost;
        survivorsNeeded = missionConfig.ConfigForSurvivors.RequiredSurvivors;
    }

    public void UpdatePanel(ActiveMission activeMission, int currentEnergy, int currentSurvivors)
    {
        if (activeMission.IsCompleted)
        {
            if (currentState != MissionPanelState.Complited)
                ChangeState(MissionPanelState.Complited);
        }
        else if (activeMission.IsFinished)
        {
            if (currentState != MissionPanelState.Finished)
                ChangeState(MissionPanelState.Finished);
        }
        else if (activeMission.IsInProgress)
        {
            if (currentState != MissionPanelState.InProgress)
                ChangeState(MissionPanelState.InProgress);

            UpdateMissionTimer(activeMission.GetMissionRemainTime());
        }
        else
        {
            if (currentState != MissionPanelState.Await)
                ChangeState(MissionPanelState.Await);

            UpdateButtonsInteractable(currentEnergy, currentSurvivors);
        }
    }

    private void ChangeState(MissionPanelState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case MissionPanelState.Await:
                ShowCanvasGroup(awaitCanvasGroup);
                HideCanvasGroup(finishedCanvasGroup);
                HideCanvasGroup(inProgressCanvasGroup);
                break;
            case MissionPanelState.InProgress:
                ShowCanvasGroup(inProgressCanvasGroup);
                HideCanvasGroup(finishedCanvasGroup);
                HideCanvasGroup(awaitCanvasGroup);
                break;
            case MissionPanelState.Finished:
                ShowCanvasGroup(finishedCanvasGroup);
                HideCanvasGroup(inProgressCanvasGroup);
                HideCanvasGroup(awaitCanvasGroup);
                break;
            case MissionPanelState.Complited:
                HideCanvasGroup(finishedCanvasGroup);
                HideCanvasGroup(inProgressCanvasGroup);
                HideCanvasGroup(awaitCanvasGroup);

                mainCanvasGroup.alpha = 0.5f;
                mainCanvasGroup.interactable = false;
                break;
            default:
                break;
        }
    }

    private void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void UpdateMissionTimer(float remainTime)
    {
        timerText.text = TimeSpan.FromSeconds(remainTime).ToString(@"hh\:mm\:ss");
    }

    private void UpdateButtonsInteractable(int energyCount, int survivorsCount)
    {
        var isHaveEnergy = energyCount >= cost;
        var isHaveSurvivors = survivorsCount >= survivorsNeeded;

        UpdateButtonCollor(isHaveEnergy, startMissionButton);
        UpdateButtonCollor(isHaveSurvivors, sendSurvivorsButton);
    }

    private void SendOnMissionClick()
    {
        OnStartMissionClick?.Invoke(this, false);
    }

    private void StartMissionClick()
    {
        OnStartMissionClick?.Invoke(this, true);
    }

    private void GetMissionRewardClick()
    {
        OnFinishMissionClick?.Invoke(this);
    }

    private void UpdateButtonCollor(bool isActive, Button button)
    {
        button.image.color = isActive ? button.colors.normalColor : button.colors.disabledColor;
    }

    public void ChangePanelVisibility(bool isVisible)
    {
        mainCanvasGroup.alpha = isVisible ? 1 : 0;
        mainCanvasGroup.interactable = isVisible;
        mainCanvasGroup.blocksRaycasts = isVisible;
    }

    private void OnDestroy()
    {
        startMissionButton.onClick.RemoveListener(StartMissionClick);
        sendSurvivorsButton.onClick.RemoveListener(SendOnMissionClick);
        getRewardButton.onClick.RemoveListener(GetMissionRewardClick);
        currentState = MissionPanelState.None;
        missionID = 0;
        cost = 0;
        survivorsNeeded = 0;
    }

    private enum MissionPanelState
    {
        None = 0,
        Await = 1,
        InProgress = 2,
        Finished = 3,
        Complited = 4,
    }
}
