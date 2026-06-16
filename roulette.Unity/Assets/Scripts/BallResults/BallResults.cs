using System;
using System.Collections.Generic;
using static RoundStateManager;

public class BallResults
{
    public event Action<HitInfo> BallSelected;

    public event Action<BallInfoData> BallHovered;
    public event Action BallUnHovered;

    private readonly IRouletteWheel rouletteWheel1;
    private readonly RoundStateManager roundStateManager;
    private readonly List<HitInfo> currentBalls = new();
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager)
    {
        rouletteWheel.AllBallsStopped += AddBalls;
        rouletteWheel.BallClicked += SelectBall;

        rouletteWheel.BallHover += OnBallHover;
        rouletteWheel.BallUnHover += OnBallUnHovered;

        this.roundStateManager = roundStateManager;
        this.roundStateManager.StateChanged += OnStateChanged;

    }

    private void OnBallHover (string ballHash)
    {
        HitInfo hoveredBall = currentBalls.Find(ball => ball.BallHash == ballHash);
        if (hoveredBall == null) return;
        BallHovered?.Invoke(hoveredBall.BallData);
    }

    private void OnBallUnHovered(string ballHash)
    {
        BallUnHovered?.Invoke();
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