using System;
using System.Collections.Generic;

public class BetCalculator
{
    public Action OnBetFailed;

    private const int ZeroMultiplier = 10;
    private const int NumMultiplier = 5;
    private const int DozenMultiplier = 3;
    private const int ColorMultiplier = 2;

    private readonly Dictionary<BetInfo, int> bets = new();

    public void MakeBet(BetInfo betInfo, int amount)
    {
        if (!bets.ContainsKey(betInfo))
            bets.Add(betInfo, 0);

        bets[betInfo] = amount;
    }

    public int RemoveBet(BetInfo betInfo, int amount)
    {
        if (!bets.ContainsKey(betInfo))
            return 0;

        var takenBet = Math.Max(bets[betInfo], amount);
        
        bets[betInfo] -= amount;
        if (bets[betInfo] <= 0)
            bets.Remove(betInfo);

        return takenBet;
    }

    public int ClearBets()
    {
        var totalCleared = 0;
        foreach (var bet in bets)
            totalCleared += bet.Value;

        bets.Clear();

        return totalCleared;
    }

    public int GetReward(HitInfo hitInfo)
    {
        var reward = 0;

        foreach (var betInfo in bets.Keys)
            reward += GetRewardForBet(betInfo, bets[betInfo], hitInfo);

        return reward;
    }

    private int GetRewardForBet(BetInfo betInfo, int amount, HitInfo hitInfo)
    {
        if (betInfo.FieldNumber == hitInfo.FieldNumber)
        {
            if (betInfo.FieldNumber == 0)
                return amount * ZeroMultiplier;

            return amount * NumMultiplier;
        }

        if (betInfo.Dozen == hitInfo.Dozen)
            return amount * DozenMultiplier;

        if (betInfo.FieldColor == hitInfo.FieldColor)
            return amount * ColorMultiplier;

        OnBetFailed?.Invoke();
        return amount;
    }
}