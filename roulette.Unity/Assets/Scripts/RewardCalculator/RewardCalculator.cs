using System;

public class RewardCalculator
{
    private readonly RoundManager roundManager;
    private readonly GoldenChips goldenChips;

    public RewardCalculator(RoundManager roundManager, GoldenChips goldenChips)
    {
        this.roundManager = roundManager;
        this.goldenChips = goldenChips;
        roundManager.Win += OnWin;
    }

    private void OnWin()
    {
        int reward = CalculateReward(roundManager.BetCount, roundManager.Quote, roundManager.Goal);
        goldenChips.Add(reward);
    }

    public int CalculateReward(int betCount, int quote, int goal)
    {
        var reward = 3;

        reward += 2 * betCount;

        var x = quote / goal;
        reward += Math.Min(x - 1, 9);

        return reward;
    }
}