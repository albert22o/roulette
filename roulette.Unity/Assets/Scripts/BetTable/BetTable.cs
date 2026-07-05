using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RoundStateManager;

public class BetTable
{
    public event Action<int, int> ChangeAmountByIndex;
    public event Action<int> ChangeAvailableSum;

    public IEnumerable<(BetInfo, int)> NotNullBetAmounts => betAmounts
        .Where(bet => bet.Value != 0)
        .Select(bet => (tableInfo.PocketMapping[bet.Key], bet.Value));

    public int AvailableSum => availableBetSum;

    private readonly HashSet<int> betIndexSet = new();
    private readonly TableInfo tableInfo;
    private readonly Dictionary<int, int> betAmounts;
    private int availableBetSum;
    private bool active;
    public const int BetStep = 25;

    public BetTable(TableInfo tableInfo, RoundStateManager roundStateManager)
    {
        this.tableInfo = tableInfo;
        betAmounts = tableInfo.PocketMapping
            .ToDictionary(pocket => pocket.Key, _ => 0);
        availableBetSum = tableInfo.StartSum;

        roundStateManager.StateChanged += OnStateChanged;
    }

    public void AddBet(int index)
    {
        if (!active)
            return;
        if (!betAmounts.ContainsKey(index))
            return;
        if (availableBetSum < BetStep)
            return;
        if (!betIndexSet.Contains(index) 
            && betIndexSet.Count >= tableInfo.MaxBetsAtBoard)
            return;
        betIndexSet.Add(index);

        betAmounts[index] += BetStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        availableBetSum -= BetStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void RemoveBet(int index) 
    {
        if (!active)
            return;
        if (!betAmounts.ContainsKey(index))
            return;
        if (betAmounts[index] < BetStep)
            return;
        betIndexSet.Remove(index);

        betAmounts[index] -= BetStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        availableBetSum += BetStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void ClearBets()
    {
        var keys = betAmounts.Keys.ToList();
        betIndexSet.Clear();
        foreach(var betIndex in keys)
        {
            availableBetSum += betAmounts[betIndex];
            betAmounts[betIndex] = 0;
            ChangeAmountByIndex?.Invoke(betIndex, 0);
        }
        ChangeAvailableSum?.Invoke(availableBetSum);
    }


    private void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.PlacingBets;

        if (newState == RoundState.Start)
        {
            availableBetSum = tableInfo.StartSum;
            ChangeAvailableSum?.Invoke(availableBetSum);
        }
    }


    public void AddAvailableSum(int amount)
    {
        availableBetSum += amount;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void RemoveAvailableSum(int amount)
    {
        availableBetSum = Mathf.Max(0, availableBetSum - amount);
        ChangeAvailableSum?.Invoke(availableBetSum);
    }
}