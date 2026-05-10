using System;
using System.Collections.Generic;
using static RoundStateManager;

public class BallResults
{
    public event Action<VisualComponent, HitInfo> BallAdded;
    public event Action<VisualComponent, HitInfo> BallRemoved;

    public event Action<HitInfo> BallSelected;

    private readonly Inventory inventory;
    private readonly RoundStateManager roundStateManager;
    private readonly List<HitInfo> currentBalls = new();
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, Inventory inventory, RoundStateManager roundStateManager)
    {
        rouletteWheel.AllBallsStopped += AddBalls;
        this.inventory = inventory;
        this.roundStateManager = roundStateManager;
        this.roundStateManager.StateChanged += OnStateChanged;
    }

    public void SelectBall(HitInfo hitInfo)
    {
        if (!active)
            return;
        BallSelected?.Invoke(hitInfo);
        RemoveBalls();
    }

    private void RemoveBalls()
    {
        foreach (var ball in currentBalls) 
        {
            // TODO: var visualComponent = inventory.FindBall(ball.BallHash);
            VisualComponent visualComponent = null;
            BallRemoved?.Invoke(visualComponent, ball);
        }
    }

    private void AddBalls(HitInfo[] balls)
    {
        roundStateManager.ChangeState(BetState.ChoosingReward);
        SelectBall(balls[0]);
        return;
        foreach (var ball in balls) 
        {
            // TODO: var visualComponent = inventory.FindBall(ball.BallHash);
            VisualComponent visualComponent = null;
            BallAdded?.Invoke(visualComponent, ball);
            currentBalls.Add(ball);
        }
    }

    public void OnStateChanged(BetState newState)
    {
        active = newState == BetState.ChoosingReward;
    }
}