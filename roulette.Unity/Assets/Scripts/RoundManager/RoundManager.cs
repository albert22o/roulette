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
    public event Action Lose;
    public event Action Rerolled;
    public event Action Spun;

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

    public float GoalModifier => goalModifier;
    public bool CanReroll => rerollCount > 0 && betState == RoundState.ChoosingReward;

    private int betCount = 3;
    private int rerollCount = 5;
    private int quote = 0;
    private int goal = 1000;
    private float goalModifier = 1f;

    private RoundState betState = RoundState.PlacingBets;
    private bool ableToWin;
    private readonly ProgressionManager progressionManager;
    private readonly BetCalculator betCalculator;
    private readonly IRouletteWheel rouletteWheel;
    private readonly RoundStateManager roundStateManager;
    private readonly TableInfo tableInfo;
    private readonly BallHolder ballHolder;
    private readonly BetTable betTable;

    public RoundManager(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager,
        TableInfo tableInfo, ProgressionManager progressionManager, BetCalculator betCalculator,
        BallHolder ballHolder, BetTable betTable)
    {
        this.rouletteWheel = rouletteWheel;
        this.betTable = betTable;
        this.roundStateManager = roundStateManager;
        this.tableInfo = tableInfo;
        this.progressionManager = progressionManager;
        this.betCalculator = betCalculator;
        this.ballHolder = ballHolder;
        betCalculator.GetReward += OnGetReward;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void Reset()
    {
        Quote = 0;
        BetCount = tableInfo.BetCount;
        RerollCount = tableInfo.RerollCount;
        Goal = (int)Math.Round(progressionManager.Current.Goal * GoalModifier);
        AbleToWin = false;
    }

    public void ModifiGoalModifier(float delta)
    {
        goalModifier += delta;
    }

    public void Spin()
    {
        if (betState != RoundState.PlacingBets)
            return;
        if (betCount <= 0)
            return;
        if (!betTable.NotNullBetAmounts.Any())
            return;

        betCalculator.ResetRerollCoeff();
        Spun?.Invoke();
        roundStateManager.ChangeState(RoundState.WheelSpining);

        rouletteWheel.ClearBalls();

        foreach (var ballInfo in ballHolder.Balls)
        {
            rouletteWheel.AddBall(ballInfo.Hash, ballInfo.BallInfoData.VisualComponent);
        }

        rouletteWheel.Spin();
        betCount--;
        BetCountChanged?.Invoke(betCount);
    }


    public void Reroll()
    {
        if (CanReroll == false) return;

        betCalculator.DecreaseRerollCoeff();
        Rerolled?.Invoke();
        rouletteWheel.ClearBalls();
        roundStateManager.ChangeState(RoundState.WheelSpining);

        foreach(var ballInfo in ballHolder.Balls)
        {
            rouletteWheel.AddBall(ballInfo.Hash, ballInfo.BallInfoData.VisualComponent);
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
        progressionManager.Advance();
    }

    public void ForceWin()
    {
        Win?.Invoke();
    }

    private void OnGetReward(int reward)
    {
        quote += reward;
        QuoteChanged?.Invoke(quote);
        if (quote >= goal)
            AbleToWin = true;
        else if (betCount <= 0)
        {
            Lose?.Invoke();
            return;
        }

        betTable.ClearBets();

        ballHolder.RemoveSelectedBallsEffect();
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