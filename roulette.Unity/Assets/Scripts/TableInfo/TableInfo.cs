using System;
using System.Collections.Generic;
using UnityEditor;

public class TableInfo
{
    public event Action<int, ColorType> PocketColorChanged;
    public event Action<int, int> PocketNumberChanged;

    public int MaxBetsAtBoard { get; }
    public int StartSum => Math.Max(0, startSum);
    public IReadOnlyDictionary<int, BetInfo> PocketMapping => pocketMapping;

    public int BetCount { get; private set; }
    public int RerollCount { get; }

    private readonly Dictionary<int, BetInfo> pocketMapping;
    private int startSum;
    private PocketData[] startPockets;

    public event Action<int> StartSumChanged;

    public TableInfo(TableConfig config)
    {
        startSum = config.StartSum;
        StartSumChanged?.Invoke(startSum);
        startPockets = config.Pockets;
        MaxBetsAtBoard = config.MaxBetsAtBoard;
        BetCount = config.BetCount;
        RerollCount = config.RerollCount;

        pocketMapping = BuildPocketMapping();
    }

    public void SetPocketColor(int index, ColorType color)
    {
        if (pocketMapping.TryGetValue(index, out var pocket))
        {
            pocket.FieldColor = color;
            PocketColorChanged?.Invoke(index, color);
        }
    }

    public void ResetPocketColor(int index)
    {
        SetPocketColor(index, startPockets[index].Color);
    }

    public void SetPocketNum(int index, int num)
    {
        if (pocketMapping.TryGetValue(index, out var pocket))
        {
            pocket.FieldNumber = num;
            PocketNumberChanged?.Invoke(index, num);
        }
    }

    public void AddToStartSum(int amount)
    {
        startSum += amount;
        StartSumChanged?.Invoke(startSum);
    }

    public void AddToBetCount(int amount) => BetCount += amount;
    public void RemoveFromStartSum(int amount) => startSum -= amount;

    private Dictionary<int, BetInfo> BuildPocketMapping()
    {
        var mapping = new Dictionary<int, BetInfo>(startPockets.Length);

        for (int i = 0; i < startPockets.Length; i++)
        {
            var pocket = startPockets[i];
            mapping.Add(i, new BetInfo(pocket.Number, pocket.Dozen, pocket.Color, i));
        }
        return mapping;
    }
}