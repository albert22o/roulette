using System;
using System.Collections.Generic;
using Zenject; 

public class BallHolder 
{
    public event Action<int, BallInfo> BallAdded;
    public event Action<int> BallRemoved;

    public IReadOnlyList<BallInfo> Balls => balls;

    public int Capacity { get; }

    private readonly List<BallInfo> balls = new();
    private readonly BallFactory ballFactory;
    private BallInfo selectedBall;

    public bool HasFreeSlot => balls.Count < Capacity;

    [Inject]
    public BallHolder(BallFactory ballFactory, BallHolderConfig config)
    {
        this.ballFactory = ballFactory;
        Capacity = config.Capacity;

        foreach (BallInfoData ball in config.StartBalls)
        {
            var ballInfo = ballFactory.Create(ball);
            balls.Add(ballInfo);
            ballInfo.OnAddedToHolder();
        }
    }

    public void AddBall(BallInfoData ballInfoData)
    {
        if (Capacity == balls.Count) return;

        var ballInfo = ballFactory.Create(ballInfoData);
        balls.Add(ballInfo);
        ballInfo.OnAddedToHolder();

        BallAdded?.Invoke(balls.Count -1, ballInfo);
    }

    public void RemoveBall(int index)
    {
        if (!IsIndexValid(index))
            return;


        balls[index].OnRemovedFromHolder();
        balls.RemoveAt(index);
        BallRemoved?.Invoke(index);
    }

    public void RemoveBall(string ballHash)
    {
        if (string.IsNullOrEmpty(ballHash)) return;

        int index = balls.FindIndex(b => b != null && b.Hash == ballHash);
        if (index != -1)
        {
            RemoveBall(index);
        }
    }

    private bool IsIndexValid(int index)
    {
        return index >= 0 && index < balls.Count;
    }

    public void ApplySelectedBall(string ballHash, HitInfo hitInfo)
    {
        BallInfo targetBall = balls.Find(b => b != null && b.Hash == ballHash);
        if (targetBall != null)
        {
            targetBall.ApplySelectedEffect(hitInfo);
            selectedBall = targetBall;
        }
    }

    public void RemoveSelectedBallEffect()
    {
        selectedBall.RemoveSelectedEffect();
    }
}
