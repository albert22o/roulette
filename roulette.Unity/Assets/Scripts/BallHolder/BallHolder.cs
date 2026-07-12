using System;
using System.Collections.Generic;

public class BallHolder
{
    public event Action<int, BallInfo> BallAdded;
    public event Action<int> BallRemoved;
    public event Action<int> CapacityChanged;
    public IReadOnlyList<BallInfo> Balls => balls;
    public int Capacity { get; private set; }
    public int FreeSlotsCount => Capacity - balls.Count;
    private readonly List<BallInfo> balls = new();
    private readonly BallFactory ballFactory;
    private bool isActive;
    public bool HasFreeSlot => balls.Count < Capacity;

    public BallHolder(GameStateManager gameStateManager, BallFactory ballFactory, BallHolderConfig config)
    {
        this.ballFactory = ballFactory;
        Capacity = config.Capacity;
        gameStateManager.GameStateChanged += OnStateChanged;
        foreach (BallInfoData ball in config.StartBalls)
        {
            var ballInfo = ballFactory.Create(ball);
            balls.Add(ballInfo);
        }
    }

    public void AddBall(BallInfoData ballInfoData)
    {
        if (!isActive || Capacity == balls.Count) return;
        var ballInfo = ballFactory.Create(ballInfoData);
        balls.Add(ballInfo);
        BallAdded?.Invoke(balls.Count - 1, ballInfo);
    }

    public void RemoveBall(int index)
    {
        if (!isActive || !IsIndexValid(index))
            return;

        balls.RemoveAt(index);
        BallRemoved?.Invoke(index);
    }

    public void ChangeCapacity(int delta)
    {
        if (delta == 0)
            return;

        int newCapacity = Capacity + delta;
        if (newCapacity < 0)
            newCapacity = 0;

        if (newCapacity == Capacity)
            return;

        while (balls.Count > newCapacity)
        {
            int lastIndex = balls.Count - 1;
            balls.RemoveAt(lastIndex);
            BallRemoved?.Invoke(lastIndex);
        }

        Capacity = newCapacity;
        CapacityChanged?.Invoke(Capacity);
    }

    private bool IsIndexValid(int index)
    {
        return index >= 0 && index < balls.Count;
    }

    private void OnStateChanged(GameStateManager.GameState newState)
    {
        isActive = newState == GameStateManager.GameState.Shopping;
    }
}