using System;
using UnityEngine;
using static RoundStateManager;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

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

    public RewardCalculator(RoundManager roundManager, GoldenChips goldenChips,
        RoundStateManager roundStateManager, BallResults ballResults)
    {
        this.roundManager = roundManager;
        this.goldenChips = goldenChips;
        this.roundStateManager = roundStateManager;
        ballResults.Win += OnWin;
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
    }

    public void CalculateReward(int betCount, int quote, int goal)
    {
        UnusedBets = betCount;
        UnusedBetsReward = unusedBetMultiplier * betCount;

        QuotaExceed = quote / goal;
        GoalBeaten?.Invoke(QuotaExceed);

        QuotaExceededReward = Math.Min(Mathf.FloorToInt(QuotaExceed) - 1, 9);

        TotalReward = BaseReward + UnusedBetsReward + QuotaExceededReward + BonusReward;

        RewardCalculated?.Invoke();
    }
}