using System;
using static RoundStateManager;

public class BetCalculator
{
    public event Action<int> GetReward;

    private const int ZeroMultiplier = 10;
    private const int NumMultiplier = 5;
    private const int DozenMultiplier = 3;
    private const int ColorMultiplier = 2;
    private bool active = true;

    private readonly BetTable betTable;
    private readonly RoundStateManager roundStateManager;
    private readonly BallResults ballResults;

    public BetCalculator(BallResults ballResults, BetTable betTable, RoundStateManager roundStateManager)
    {
        this.ballResults = ballResults;
        ballResults.BallSelected += OnBallSelected;
        
        this.betTable = betTable;
        this.roundStateManager = roundStateManager;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void OnStateChanged(BetState newState)
    {
        active = newState == BetState.ChoosingReward;
    }

    private void OnBallSelected(HitInfo info)
    {
        if (!active)
            return;

        var reward = 0;
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts) 
        {
            reward += GetRewardForBet(betInfo, amount, info);
        }
        betTable.ClearBets();

        GetReward?.Invoke(reward);
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

        return amount;
    }
}