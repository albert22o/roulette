using System;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class BallHolder 
{
    public event Action<int, BallInfo> BallAdded;
    public event Action<int> BallRemoved;
    public event Action<int, int> BallsSwapped;

    public int Capacity { get; }

    private readonly BallInfo[] balls;
    private bool isActive = false;
    private readonly RoundStateManager roundStateManager;

    [Inject]
    public BallHolder(RoundStateManager roundStateManager, int capacity = 5)
    {
        this.roundStateManager = roundStateManager;
        Capacity = capacity;

        balls = new BallInfo[capacity];

        roundStateManager.StateChanged += OnStateChanged;
    }

    public void AddBall(BallInfo ballinfo)
    {
        if (!isActive) return;

        int freeIndex = FindFreeSlot();
        if(freeIndex == -1) return;

        balls[freeIndex] = ballinfo;
        BallAdded?.Invoke(freeIndex, ballinfo);
    }

    public void RemoveBall(int index)
    {
        if (!IsValidIndex(index) || balls[index] == null)
            return;
        balls[index] = null;
        BallRemoved?.Invoke(index);
    }
    
    public void SwapBalls(int indexA, int indexB)
    {
        if (!isActive || !IsValidIndex(indexA) || !IsValidIndex(indexB) || indexA == indexB)
            return;
        (balls[indexA], balls[indexB]) = (balls[indexB], balls[indexA]);
        BallsSwapped?.Invoke(indexA, indexB);
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        isActive = newState == RoundStateManager.RoundState.PlacingBets;
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i] == null) return i;
        }
        return -1;
    }

    private bool IsValidIndex(int index) => index >= 0 && index < balls.Length;

}
