using System;

public class RoundManager
{
    public event Action<int> BetCountChanged;
    public event Action<int> RerollCountChanged;
    public event Action<int> QuoteChanged;

    public event Action Win;
    public event Action Lose;     

    public bool IsRerollAvailable => rerollCount > 0 && betState == BetState.TakingReward;
    public bool IsSpinAvailable => betCount > 0 && betState == BetState.PlacingBets;
    public int Goal => goal;
    public int Quote => quote;
    public int BetCount => betCount;
    public int RerollCount => rerollCount;

    private int betCount = 3;
    private int rerollCount = 5;
    private int quote = 0;
    private int goal = 1000;

    private BetState betState = BetState.PlacingBets;
    private readonly IRouletteWheel rouletteWheel;
    private readonly BetCalculator betCalculator;

    public RoundManager(IRouletteWheel rouletteWheel, BetCalculator betCalculator)
    {
        this.rouletteWheel = rouletteWheel;
        this.betCalculator = betCalculator;
        betCalculator.GetReward += OnGetReward;
    }

    public void Spin()
    {
        if (betState != BetState.PlacingBets)
            return;
        if (betCount <= 0)
            return;

        ChangeState(BetState.TakingReward);
        rouletteWheel.Spin();
        betCount--;
        BetCountChanged?.Invoke(betCount);
    }

    public void Reroll()
    {
        if (betState != BetState.TakingReward)
            return;
        if (rerollCount <= 0)
            return;

        rouletteWheel.Spin();
        rerollCount--;
        RerollCountChanged?.Invoke(rerollCount);
    }

    private void OnGetReward(int reward)
    {
        quote += reward;
        if (quote >= goal)
            Win?.Invoke();
        else if (betCount <= 0)
            Lose?.Invoke();

        QuoteChanged?.Invoke(quote);
        ChangeState(BetState.PlacingBets);
    }

    public void ChangeState(BetState newState)
    {
        betState = newState;
        betCalculator.ChangeState(newState);
    }

    public enum BetState
    {
        PlacingBets,
        TakingReward,
        NotActive
    }
}