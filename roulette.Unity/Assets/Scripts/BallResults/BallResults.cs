using System;
using System.Collections.Generic;
using static RoundStateManager;

public class BallResults
{
    public event Action<HitInfo> BallSelected;

    private readonly RoundStateManager roundStateManager;
    private readonly List<HitInfo> currentBalls = new();
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager)
    {
        rouletteWheel.AllBallsStopped += AddBalls;
        rouletteWheel.BallClicked += SelectBall;
        this.roundStateManager = roundStateManager;
        this.roundStateManager.StateChanged += OnStateChanged;
    }

    public void SelectBall(string ballhash)
    {
        if (!active)
            return;
        var hitInfo = currentBalls.Find(ball => ball.BallHash == ballhash );

        BallSelected?.Invoke(hitInfo);
        RemoveBalls();
    }

    private void RemoveBalls()
    {
        currentBalls.Clear();
    }

    private void AddBalls(HitInfo[] balls)
    {
        roundStateManager.ChangeState(RoundState.ChoosingReward);
        currentBalls.AddRange(balls);
    }

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }
}