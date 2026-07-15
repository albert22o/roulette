using System;
using System.Collections.Generic;
using System.Linq;
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
    private BetTable betTable;

    public bool RandomBallSelection { get; set; }

    public BallResults(IRouletteWheel rouletteWheel, RoundStateManager roundStateManager,
        BallHolder ballHolder, TableInfo tableInfo, BetTable betTable)
    {
        this.ballHolder = ballHolder;
        this.tableInfo = tableInfo;
        rouletteWheel.AllBallsStopped += AddBalls;
        rouletteWheel.BallClicked += SelectBall;

        rouletteWheel.BallHover += OnBallHover;
        rouletteWheel.BallUnHover += OnBallUnHovered;

        this.roundStateManager = roundStateManager;
        this.roundStateManager.StateChanged += OnStateChanged;
        this.betTable = betTable;
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
        RemoveBalls();

        if (hitInfo.FieldColor == ColorType.White)
        {
            tableInfo.ResetPocketColor(hitInfo.FieldIndex);
            betTable.ClearBets();
            Win?.Invoke();
            return;
        }
        BallSelected?.Invoke(hitInfo);
    }

    private void RemoveBalls()
    {
        currentBalls.Clear();
    }

    private void AddBalls(HitInfo[] balls)
    {
        roundStateManager.ChangeState(RoundState.ChoosingReward);
        currentBalls.Clear();
        currentBalls.AddRange(balls);
        if (balls.Any(ball => ball.FieldColor == ColorType.Grey))
        {
            // Отправляем выбор нулевого шара для проигрыша
            BallSelected?.Invoke(null);
            return;
        }
        if (RandomBallSelection)
        {
            var hash = balls[UnityEngine.Random.Range(0, balls.Length)].BallHash;
            SelectBall(hash);
            return;
        }
    }

    public void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.ChoosingReward;
    }
}