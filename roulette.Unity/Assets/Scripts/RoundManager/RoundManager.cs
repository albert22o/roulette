using System;
using UnityEngine;
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
    private readonly TableConfig tableConfig;

    public RoundManager(IRouletteWheel rouletteWheel, BetCalculator betCalculator, RoundStateManager roundStateManager,
        TableConfig tableConfig, ProgressionManager progressionManager, BetResultsDisplay betResultsDisplay)
    {
        this.rouletteWheel = rouletteWheel;

        betResultsDisplay.GetReward += OnGetReward;
        
        this.roundStateManager = roundStateManager;
        this.tableConfig = tableConfig;
        this.progressionManager = progressionManager;
        roundStateManager.StateChanged += OnStateChanged;
    }

    public void Reset()
    {
        Quote = 0;
        BetCount = tableConfig.BetCount;
        RerollCount = tableConfig.RerollCount;
        Goal = progressionManager.Current.Goal;
        AbleToWin = false;
    }

    public void Spin()
    {
        if (betState != RoundState.PlacingBets)
            return;
        if (betCount <= 0)
            return;

        roundStateManager.ChangeState(RoundState.WheelSpining);

        rouletteWheel.ClearBalls();
        // Убрать потом
        var dummyMaterial = new Material(Shader.Find("Standard"));
        var visualComponent = new VisualComponent()
        {
            Material = dummyMaterial
        };
        rouletteWheel.AddBall("1", visualComponent);

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
        // Убрать потом
        var dummyMaterial = new Material(Shader.Find("Standard"));
        var visualComponent = new VisualComponent()
        {
            Material = dummyMaterial
        };
        rouletteWheel.AddBall("1", visualComponent);

        rouletteWheel.Spin();
        rerollCount--;
        RerollCountChanged?.Invoke(rerollCount);
    }

    public void ConfirmWin()
    {
        if (!AbleToWin)
            return;
        Win?.Invoke();
        roundStateManager.ChangeState(RoundState.Win);
    }

    private void OnGetReward(int reward)
    {
        quote += reward;
        if (quote >= goal)
            AbleToWin = true;
        else if (betCount <= 0)
            roundStateManager.ChangeState(RoundState.Lose);

        QuoteChanged?.Invoke(quote);
        roundStateManager.ChangeState(RoundState.PlacingBets);
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