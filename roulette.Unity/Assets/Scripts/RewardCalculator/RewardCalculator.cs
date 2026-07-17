using System;
using static RoundStateManager;

public class RewardCalculator
{
    public int BaseReward { get; set; } = 3;
    public Action<int> GoalBeaten;

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
        int reward = CalculateReward(roundManager.BetCount, roundManager.Quote, roundManager.Goal);
        goldenChips.Add(reward);
    }

    public int CalculateReward(int betCount, int quote, int goal)
    {
        var reward = BaseReward;

        reward += 2 * betCount;

        var x = quote / goal;

        GoalBeaten?.Invoke(x);

        reward += Math.Min(x - 1, 9);

        return reward;
    }
}