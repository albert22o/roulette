using System;
using System.Collections.Generic;
using System.Linq;
using static RoundStateManager;

public class BallResults
{
    public event Action<HitInfo> BallSelected;

    public event Action<BallInfoData> BallHovered;
    public event Action BallUnHovered;

    private readonly RoundStateManager roundStateManager;
    private readonly BallHolder ballHolder;
    private readonly List<HitInfo> currentBalls = new();
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager, BallHolder ballHolder)
    {
        this.ballHolder = ballHolder;
        rouletteWheel.AllBallsStopped += AddBalls;
        rouletteWheel.BallClicked += SelectBall;

        rouletteWheel.BallHover += OnBallHover;
        rouletteWheel.BallUnHover += OnBallUnHovered;

        this.roundStateManager = roundStateManager;
        this.roundStateManager.StateChanged += OnStateChanged;

    }

    private void OnBallHover(string ballHash)
    {
        if (!active) return;

        BallInfo hoveredBall = ballHolder.Balls.FirstOrDefault(ball => ball.BallHash == ballHash);
        if (hoveredBall == null) return;
        BallInfoData ballData = hoveredBall.BallInfoData;
        BallHovered?.Invoke(ballData);
    }

    private void OnBallUnHovered(string ballHash)
    {
        BallUnHovered?.Invoke();
    }

    public void SelectBall(string ballhash)
    {
        if (!active)
            return;
        var hitInfo = currentBalls.Find(ball => ball.BallHash == ballhash);

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