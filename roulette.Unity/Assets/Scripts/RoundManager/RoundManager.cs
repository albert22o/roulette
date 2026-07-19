using System;
using System.Linq;
using static RoundStateManager;

public class RoundManager
{
    public event Action<int> BetCountChanged;
    public event Action<int> RerollCountChanged;
    public event Action<int> QuoteChanged;
    public event Action<int> GoalChanged;
    public event Action<bool> AbleToWinChanged;

    public event Action Win;

    public int Goal
    {
        get => goal;
        set
        {
            goal = value;
            GoalChanged?.Invoke(goal);
        }
    }
    
    public int BetCount
    {
        get => betCount;
        private set
        {
            betCount = value;
            BetCountChanged?.Invoke(betCount);
        }
    }

    public int RerollCount
    {
        get => rerollCount;
        private set
        {
            rerollCount = value;
            RerollCountChanged?.Invoke(rerollCount);
        }
    }

    public int Quote
    {
        get => quote;
        private set
        {
            quote = value;
            QuoteChanged?.Invoke(quote);
        }
    }

    public bool AbleToWin {
        get => ableToWin; 
        set
            {
                ableToWin = value;
                AbleToWinChanged?.Invoke(value);
            }
        }

    private int betCount = 3;
    private int rerollCount = 5;
    private int quote = 0;
    private int goal = 1000;

    private RoundState betState = RoundState.PlacingBets;
    private bool ableToWin;
    private readonly ProgressionManager progressionManager;
    private readonly IRouletteWheel rouletteWheel;
    private readonly RoundStateManager roundStateManager;
    private readonly TableInfo tableInfo;
    private readonly ChipHolder chipHolder;
    private readonly BallHolder ballHolder;
    private readonly BetTable betTable;

    public RoundManager(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager,
        TableInfo tableInfo, ProgressionManager progressionManager, BetCalculator betCalculator,
        ChipHolder chipHolder, BallHolder ballHolder, BetTable betTable)
    {
        this.rouletteWheel = rouletteWheel;

        betCalculator.GetReward += OnGetReward;

        this.betTable = betTable;
        this.roundStateManager = roundStateManager;
        this.tableInfo = tableInfo;
        this.progressionManager = progressionManager;
        this.chipHolder = chipHolder;
        this.ballHolder = ballHolder;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void Reset()
    {
        Quote = 0;
        BetCount = tableInfo.BetCount;
        RerollCount = tableInfo.RerollCount;
        Goal = progressionManager.Current.Goal;
        AbleToWin = false;
    }

    public void Spin()
    {
        if (betState != RoundState.PlacingBets)
            return;
        if (betCount <= 0)
            return;
        if (!betTable.NotNullBetAmounts.Any())
            return;

        roundStateManager.ChangeState(RoundState.WheelSpining);

        rouletteWheel.ClearBalls();

        foreach (var ballInfo in ballHolder.Balls)
        {
            rouletteWheel.AddBall(ballInfo.Hash, ballInfo.BallData.VisualComponent);
        }

        rouletteWheel.Spin();
        betCount--;
        BetCountChanged?.Invoke(betCount);
    }

    public void Reroll()
    {
        if (betState != RoundState.ChoosingReward)
            return;
        if (rerollCount <= 0)
            return;

        rouletteWheel.ClearBalls();
        roundStateManager.ChangeState(RoundState.WheelSpining);

        foreach(var ballInfo in ballHolder.Balls)
        {
            rouletteWheel.AddBall(ballInfo.Hash, ballInfo.BallData.VisualComponent);
        }
        
        rouletteWheel.Spin();
        rerollCount--;
        RerollCountChanged?.Invoke(rerollCount);
    }

    public void ConfirmWin()
    {
        if (!AbleToWin)
            return;
        Win?.Invoke();
    }

    private void OnGetReward(int reward)
    {
        quote += reward;
        if (quote >= goal)
            AbleToWin = true;
        else if (betCount <= 0)
            roundStateManager.ChangeState(RoundState.Lose);

        betTable.ClearBets();
        chipHolder.RevertAll();

        ballHolder.RemoveSelectedBallEffect();
        QuoteChanged?.Invoke(quote);
        roundStateManager.ChangeState(RoundState.PlacingBets);
    }

    public void AddRerollCount(int amount)
    {
        rerollCount += amount;
        RerollCountChanged?.Invoke(rerollCount);
    }

    private void OnStateChanged(RoundState newState)
    {
        betState = newState;
        if (newState == RoundState.Start)
        {
            Reset();
            roundStateManager.ChangeState(RoundState.PlacingBets);
        }
    }
}