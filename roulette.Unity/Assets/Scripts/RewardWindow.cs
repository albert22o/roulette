using System;
using TMPro;
using UnityEngine;
using Zenject;

public class RewardWindow : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private TextMeshProUGUI quota;
    [SerializeField] private TextMeshProUGUI unusedBetsCount, unusedBetsBonus;
    [SerializeField] private TextMeshProUGUI quotaExceededValue, quotaExceededBonus;
    [SerializeField] private TextMeshProUGUI additionalBonus;
    [SerializeField] private TextMeshProUGUI total;
    [SerializeField] private GameObject unusedBetsRow, quotaExceededRow, additionalBonusRow;

    private RewardCalculator rewardCalculator;
    private GameStateManager gameStateManager;

    [Inject]
    private void Construct(RewardCalculator rewardCalculator, GameStateManager gameStateManager)
    {
        this.rewardCalculator = rewardCalculator;
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        rewardCalculator.RewardCalculated += UpdateRewardWindow;
    }

    private void OnDestroy()
    {
        rewardCalculator.RewardCalculated -= UpdateRewardWindow;
    }

    public void Show()
    {
        window.SetActive(true);
    }

    public void Hide()
    {
        window.SetActive(false);
    }

    public void ApplyButtonClick()
    {
        rewardCalculator.ReceiveReward();
        Hide();
    }

    private void UpdateRewardWindow()
    {
        quota.text = rewardCalculator.BaseReward.ToString();
        if (rewardCalculator.UnusedBetsReward != 0)
        {
            unusedBetsRow.SetActive(true);
            unusedBetsCount.text = Loc.Get("reward.unused_bets_count", rewardCalculator.UnusedBets);
            unusedBetsBonus.text = rewardCalculator.UnusedBetsReward.ToString();
        }
        else
        {
            unusedBetsRow.SetActive(false);
        }
        if (rewardCalculator.QuotaExceededReward != 0)
        {
            quotaExceededRow.SetActive(true);
            quotaExceededValue.text = Loc.Get("reward.quota_exceeded_value", rewardCalculator.QuotaExceed);
            quotaExceededBonus.text = rewardCalculator.QuotaExceededReward.ToString();
        }
        else
        {
            quotaExceededRow.SetActive(false);
        }
        if (rewardCalculator.BonusReward != 0)
        {
            additionalBonusRow.SetActive(true);
            additionalBonus.text = rewardCalculator.BonusReward.ToString();
        }
        else
        {
            additionalBonusRow.SetActive(false);
        }
        
        total.text = rewardCalculator.TotalReward.ToString();
        Show();
    }
}
