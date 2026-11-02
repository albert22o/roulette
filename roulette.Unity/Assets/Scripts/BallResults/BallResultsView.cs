using System;
using TMPro;
using UnityEngine;
using Zenject;

public class BallResultsView : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private BallResults ballResults;
    private RoundStateManager roundStateManager;
    private bool isActive;
    private IRouletteWheel rouletteWheel;

    [Inject]
    public void Construct(BallResults ballResults, RoundStateManager roundStateManager, IRouletteWheel rouletteWheel)
    {
        this.ballResults = ballResults;
        this.roundStateManager = roundStateManager;
        this.rouletteWheel = rouletteWheel;

        ballResults.BallHovered += ShowInfo;
        ballResults.BallUnHovered += HideInfo;
        roundStateManager.StateChanged += OnRoundStateChanged;
        rouletteWheel.Rerolled += OnReroll;

        HideInfo();
    }

    private void OnDestroy()
    {
        ballResults.BallHovered -= ShowInfo;
        ballResults.BallUnHovered -= HideInfo;
        roundStateManager.StateChanged -= OnRoundStateChanged;
        rouletteWheel.Rerolled -= OnReroll;
    }

    private void OnReroll()
    {
        HideInfo();
    }

    private void OnRoundStateChanged(RoundStateManager.RoundState state)
    {
        if (state == RoundStateManager.RoundState.ChoosingReward)
        {
            isActive = true;
        }
        else
        {
            HideInfo();
            isActive = false;
        }
    }
    private void ShowInfo(BallInfoData ballInfoData)
    {
        if (isActive)
        {
            infoPanel.SetActive(true);
            nameText.text = ballInfoData.Name;
            descriptionText.text = ballInfoData.Description;
        }
    }

    private void HideInfo()
    {
        infoPanel.SetActive(false);
    }


}
