using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BallHolder 
{
    public event Action<int, BallInfo> BallAdded;
    public event Action<int> BallRemoved;

    public IReadOnlyCollection<BallInfo> Balls => balls;

    public int Capacity { get; }

    private readonly List<BallInfo> balls = new();
    private readonly BallFactory ballFactory;
    private bool isActive;

    [Inject]
    public BallHolder(GameStateManager gameStateManager, BallFactory ballFactory, int capacity = 5)
    {
        this.ballFactory = ballFactory;
        // Убрать потом
        var dummyMaterial = new Material(Shader.Find("Standard"));
        var visualComponent = new VisualComponent()
        {
            Material = dummyMaterial
        };

        var ballData = new SimpleBallInfoData()
        {
            VisualComponent = visualComponent
        };
        balls.Add(ballFactory.Create(ballData));
        balls.Add(ballFactory.Create(ballData));

        Capacity = capacity;

        gameStateManager.GameStateChanged += OnStateChanged;
    }

    public void AddBall(BallInfoData ballInfoData)
    {
        if (!isActive || Capacity == balls.Count) return;

        var ballInfo = ballFactory.Create(ballInfoData);
        balls.Add(ballInfo);

        BallAdded?.Invoke(balls.Count -1, ballInfo);
    }

    public void RemoveBall(int index)
    {
        if (!isActive || IsIndexValid(index))
            return;
        balls.RemoveAt(index);
        BallRemoved?.Invoke(index);
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
