using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RoundStateManager;

public class BetTable
{
    public event Action<int, int> ChangeAmountByIndex;
    public event Action<int> ChangeAvailableSum;
    public event Action<bool> HasActiveBetsChanged;

    public IEnumerable<(BetInfo, int)> NotNullBetAmounts => betAmounts
        .Where(bet => bet.Value != 0)
        .Select(bet => (tableInfo.PocketMapping[bet.Key], bet.Value));

    public int AvailableSum => availableBetSum;
    public bool HasActiveBets => betIndexSet.Count > 0;

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
        this.tableInfo.StartSumChanged += OnAvailableSumChanged;
    }

    public void AddBet(int index)
    {
        if (CanBet(index) == false) return;
        bool hadNoBetsBefore = !HasActiveBets;
        betIndexSet.Add(index);

        betAmounts[index] += BetStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        availableBetSum -= BetStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
        if (hadNoBetsBefore)
        {
            HasActiveBetsChanged?.Invoke(true);
        }
    }

    public bool CanBet(int index)
    {
        if (!active)
            return false;
        if (!betAmounts.ContainsKey(index))
            return false;
        if (tableInfo.PocketMapping[index].FieldColor == ColorType.White || 
            tableInfo.PocketMapping[index].FieldColor == ColorType.Grey)
            return false;
        if (availableBetSum < BetStep)
            return false;
        if (!betIndexSet.Contains(index)
            && betIndexSet.Count >= tableInfo.MaxBetsAtBoard)
            return false;
        return true;
    }

    public bool CanHighlight(int index)
    {
        if (CanBet(index))
            return true;
        if (betAmounts.ContainsKey(index) && betAmounts[index] > 0)
            return true;
        return false;
    }

    public void RemoveBet(int index)
    {
        if (!active)
            return;
        if (!betAmounts.ContainsKey(index))
            return;
        if (betAmounts[index] < BetStep)
            return;

        betAmounts[index] -= BetStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        if (betAmounts[index] == 0)
        {
            betIndexSet.Remove(index);
        }

        availableBetSum += BetStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
        if (!HasActiveBets)
        {
            HasActiveBetsChanged?.Invoke(false);
        }
    }

    public void ClearBets()
    {
        bool hadBetsBefore = HasActiveBets;
        var keys = betAmounts.Keys.ToList();
        betIndexSet.Clear();
        foreach (var betIndex in keys)
        {
            availableBetSum += betAmounts[betIndex];
            betAmounts[betIndex] = 0;
            ChangeAmountByIndex?.Invoke(betIndex, 0);
        }
        ChangeAvailableSum?.Invoke(availableBetSum);
        if (hadBetsBefore)
        {
            HasActiveBetsChanged?.Invoke(false);
        }
    }

    public void OnAvailableSumChanged(int newSum)
    {
        availableBetSum = newSum;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    private void OnStateChanged(RoundState newState)
    {
        active = newState == RoundState.PlacingBets;

        if (newState == RoundState.Start)
        {
            if (HasActiveBets)
            {
                betIndexSet.Clear();
                HasActiveBetsChanged?.Invoke(false);
            }
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