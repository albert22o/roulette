using System;

public class RoundManager
{
    public Action OnBetInfoChanged;
    public bool IsRerollAvailable => rerollCount > 0;
    public int BetCount => betCount;
    public int RerollCount => rerollCount;
    public int BetSum => betSum;

    private int betCount = 3;
    private int rerollCount = 5;
    private int betSum = 200;

    private readonly IRouletteWheel rouletteWheel;
    private readonly BetCalculator betCalculator = new();

    public RoundManager(IRouletteWheel rouletteWheel)
    {
        this.rouletteWheel = rouletteWheel;
    }

    public void MakeBet(BetInfo betInfo, int amount)
    {
        if (betSum < amount)
            return;

        betSum -= amount;
        betCalculator.MakeBet(betInfo, amount);
    }

    public void RemoveBet(BetInfo betInfo, int amount)
    {
        var removedBet = betCalculator.RemoveBet(betInfo, amount);
        betSum += removedBet;
    }

    public void ClearBets()
    {
        betSum += betCalculator.ClearBets();
    }

    public void ConfirmBet()
    {
        if (betCount > 0)
        {
            betCount--;
            return;
        }

        rouletteWheel.Spin();
    }

    public void Reroll()
    {
        if (rerollCount > 0)
        {
            rerollCount--;
            rouletteWheel.Spin();
        }
    }

    public int GetReward(HitInfo hitInfo)
    {
        betSum += betCalculator.ClearBets();

        return betCalculator.GetReward(hitInfo);
    }
}