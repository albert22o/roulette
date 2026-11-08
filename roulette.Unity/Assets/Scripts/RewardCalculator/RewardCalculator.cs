using System;
using UnityEngine;

public class RewardCalculator
{
    private int unusedBetMultiplier = 3;
    public int BaseReward { get; set; } = 3;
    public int UnusedBets { get; set; }
    public int UnusedBetsReward { get; set; }
    public float QuotaExceed { get; set; }
    public int QuotaExceededReward { get; set; } 
    public int BonusReward { get; private set; }
    public int TotalReward { get; set; }
    public event Action<float> GoalBeaten;
    public event Action RewardCalculated;
    public event Action RewardReceived;

    private readonly RoundManager roundManager;
    private readonly GoldenChips goldenChips;
    private readonly RoundStateManager roundStateManager;
    private readonly ProgressionManager progressionManager;

    public RewardCalculator(RoundManager roundManager, GoldenChips goldenChips,
        RoundStateManager roundStateManager, ProgressionManager progressionManager)
    {
        this.roundManager = roundManager;
        this.goldenChips = goldenChips;
        this.roundStateManager = roundStateManager;
        this.progressionManager = progressionManager;
        roundManager.Win += OnWin;
    }

    private void OnWin()
    {
        CalculateReward(roundManager.BetCount, roundManager.Quote, roundManager.Goal);
    }

    public void ReceiveReward()
    {
        RewardReceived?.Invoke();
        goldenChips.Add(TotalReward);
        progressionManager.Advance();
    }

    public void CalculateReward(int betCount, long quote, long goal)
    {
        UnusedBets = betCount;
        UnusedBetsReward = unusedBetMultiplier * betCount;

        QuotaExceed = quote / goal;
        if (QuotaExceed >= 1)
        {
            GoalBeaten?.Invoke(QuotaExceed);
            QuotaExceededReward = Math.Min(Mathf.FloorToInt(QuotaExceed) - 1, 9);
        }
        else
        {
            QuotaExceededReward = 0;
        }


        TotalReward = BaseReward + UnusedBetsReward + QuotaExceededReward + BonusReward;

        RewardCalculated?.Invoke();
    }
}