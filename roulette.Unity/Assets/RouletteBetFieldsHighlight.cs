using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RouletteBetFieldsHighlight : IInitializable, IDisposable
{
    private readonly BetTable betTable;
    private readonly RouletteWheel wheel;
    private readonly TableInfo tableInfo;

    private readonly HashSet<int> activeBetIndices = new();
    private readonly Dictionary<ColorType, int> colorBetCounts = new();
    private readonly Dictionary<DozenType, int> dozenBetCounts = new();
    private readonly Dictionary<int, int> numberBetCounts = new();

    public RouletteBetFieldsHighlight(BetTable betTable, RouletteWheel wheel, TableInfo tableInfo)
    {
        this.betTable = betTable;
        this.wheel = wheel;
        this.tableInfo = tableInfo;
    }

    public void Initialize()
    {
        betTable.ChangeAmountByIndex += OnChangeAmountByIndex;
    }

    public void Dispose()
    {
        betTable.ChangeAmountByIndex -= OnChangeAmountByIndex;
    }

    private void OnChangeAmountByIndex(int index, int betSum)
    {
        var pocketField = tableInfo.PocketMapping[index];
        int pocketNumber = pocketField.FieldNumber;
       

        bool isActive = betSum > 0;
        bool wasActive = activeBetIndices.Contains(index);
        if (isActive == wasActive) return;

        if (isActive) activeBetIndices.Add(index);
        else activeBetIndices.Remove(index);

        UpdateGroup(numberBetCounts, pocketNumber, isActive, RefreshFieldsByNumber);

        var wheelPosition = wheel.FieldsOrder[index];

        var wheelField = wheel.Fields[wheelPosition];
        UpdateGroup(colorBetCounts, pocketField.FieldColor, isActive, RefreshFieldsByColor);
        UpdateGroup(dozenBetCounts, pocketField.Dozen, isActive, RefreshFieldsByDozen);
    }

    private void UpdateGroup<TKey>(Dictionary<TKey, int> counts, TKey key, bool increase, Action<TKey> onBoundaryCrossed)
    {
        counts.TryGetValue(key, out int current);
        int updated = Mathf.Max(0, current + (increase ? 1 : -1));
        counts[key] = updated;

        bool crossedZero = (current == 0 && updated > 0) || (current > 0 && updated == 0);
        if (crossedZero) onBoundaryCrossed(key);
    }

    private void RefreshFieldsByNumber(int number)
    {
        bool shouldHighlight = numberBetCounts.TryGetValue(number, out int c) && c > 0;
        for (int i = 0; i < wheel.Fields.Length; i++)
            if (wheel.Fields[i].Number == number)
                wheel.HighlightStates[i].IsNumberBetHighlighted = shouldHighlight;
    }

    private void RefreshFieldsByColor(ColorType color)
    {
        bool shouldHighlight = colorBetCounts.TryGetValue(color, out int c) && c > 0;
        for (int i = 0; i < wheel.Fields.Length; i++)
            if (wheel.Fields[i].Color == color)
                wheel.HighlightStates[i].IsColorBetHighlighted = shouldHighlight;
    }

    private void RefreshFieldsByDozen(DozenType dozen)
    {
        bool shouldHighlight = dozenBetCounts.TryGetValue(dozen, out int c) && c > 0;
        for (int i = 0; i < wheel.Fields.Length; i++)
            if (wheel.Fields[i].Dozen == dozen)
                wheel.HighlightStates[i].IsDozenBetHighlighted = shouldHighlight;
    }
}