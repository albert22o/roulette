using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using static RoundStateManager;

public class BallResults
{
    public event Action<HitInfo> BallSelected;

    public event Action<BallInfoData> BallHovered;
    public event Action BallUnHovered;

    public event Action Win;

    private readonly RoundStateManager roundStateManager;
    private readonly BallHolder ballHolder;
    private readonly TableInfo tableInfo;
    private readonly List<HitInfo> currentBalls = new();
    private bool active;

    public BallResults(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager,
        BallHolder ballHolder, TableInfo tableInfo)
    {
        this.ballHolder = ballHolder;
        this.tableInfo = tableInfo;
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

        if (hitInfo.FieldColor == ColorType.White)
        {
            tableInfo.ResetPocketColor(hitInfo.FieldIndex);
            Win?.Invoke();
        }
        BallSelected?.Invoke(hitInfo);
        RemoveBalls();
    }

    private void RemoveBalls()
    {
        currentBalls.Clear();
    }

    private void AddBalls(HitInfo[] balls)
    {
        if (balls.Any(ball => ball.FieldColor == ColorType.Grey))
        {
            // Отправляем выбор нулевого шара для проигрыша
            BallSelected?.Invoke(null);
            return;
        }
        currentBalls.Clear();
        roundStateManager.ChangeState(RoundState.ChoosingReward);
        currentBalls.AddRange(balls);
    }

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }
}