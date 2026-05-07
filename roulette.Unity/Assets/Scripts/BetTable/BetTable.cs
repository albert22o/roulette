using System;
using System.Collections.Generic;
using System.Linq;

public class BetTable
{
    public event Action<int, int> ChangeAmountByIndex;
    public event Action<int> ChangeAvailableSum;

    public IEnumerable<(BetInfo, int)> NotNullBetAmounts => betAmounts
        .Where(bet => bet.Value != 0)
        .Select(bet => (tableConfig.PocketMapping[bet.Key], bet.Value));

    public int AvailableSum => availableBetSum;

    private readonly HashSet<int> betIndexSet = new();
    private readonly TableConfig tableConfig;
    private readonly Dictionary<int, int> betAmounts;
    private int availableBetSum;
    private bool active = true;
    private const int betStep = 25;

    public BetTable(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;

        betAmounts = tableConfig.PocketMapping.
            ToDictionary(pocket => pocket.Key, _ => 0);
        availableBetSum = tableConfig.StartSum;
    }

    public void AddBet(int index)
    {
        if (!active)
            return;
        if (!betAmounts.ContainsKey(index))
            return;
        if (availableBetSum < betStep)
            return;
        if (!betIndexSet.Contains(index) 
            && betIndexSet.Count >= tableConfig.MaxBetsAtBoard)
            return;
        betIndexSet.Add(index);

        betAmounts[index] += betStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        availableBetSum -= betStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void RemoveBet(int index) 
    {
        if (!active)
            return;
        if (!betAmounts.ContainsKey(index))
            return;
        if (betAmounts[index] < betStep)
            return;
        betIndexSet.Remove(index);

        betAmounts[index] -= betStep;
        ChangeAmountByIndex?.Invoke(index, betAmounts[index]);

        availableBetSum += betStep;
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void ClearBets()
    {
        foreach(var betIndex in betAmounts.Keys)
        {
            availableBetSum += betAmounts[betIndex];
            betAmounts[betIndex] = 0;
            ChangeAmountByIndex?.Invoke(betIndex, 0);
        }
        ChangeAvailableSum?.Invoke(availableBetSum);
    }

    public void ChangeState(RoundManager.BetState newState)
    {
        active = newState == RoundManager.BetState.PlacingBets;
    }
}