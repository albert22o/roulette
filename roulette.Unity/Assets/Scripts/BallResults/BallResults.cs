using System;
using System.Collections.Generic;

public class BallResults
{
    public event Action<VisualComponent, HitInfo> BallAdded;
    public event Action<VisualComponent, HitInfo> BallRemoved;

    public event Action<HitInfo> BallSelected;

    private readonly Inventory inventory;
    private readonly List<HitInfo> currentBalls;
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, Inventory inventory)
    {
        rouletteWheel.AllBallsStopped += AddBalls;
        this.inventory = inventory;
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
        foreach (var ball in balls) 
        {
            // TODO: var visualComponent = inventory.FindBall(ball.BallHash);
            VisualComponent visualComponent = null;
            BallAdded?.Invoke(visualComponent, ball);
            currentBalls.Add(ball);
        }
    }

    public void ChangeState(RoundManager.BetState newState)
    {
        active = newState == RoundManager.BetState.TakingReward;
    }
}