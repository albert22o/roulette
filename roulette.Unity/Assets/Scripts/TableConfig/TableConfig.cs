using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;

public class TableConfig
{
    public event Action<int, ColorType> PocketColorChanged;
    public event Action<int, int> PocketNumberChanged;

    public int MaxBetsAtBoard => maxBetsAtBoard;
    public int StartSum => Math.Max(0, startSum);
    public IReadOnlyDictionary<int, BetInfo> PocketMapping => pocketMapping;

    public int BetCount = 3;
    public int RerollCount = 5;

    private readonly Dictionary<int, BetInfo> pocketMapping = new()
    {
        { 0,  new BetInfo(0,  DozenType.Zero,  ColorType.Green) },
        { 1,  new BetInfo(1,  DozenType.First, ColorType.Red) },
        { 2,  new BetInfo(2,  DozenType.First, ColorType.Black) },
        { 3,  new BetInfo(3,  DozenType.First, ColorType.Red) },
        { 4,  new BetInfo(4,  DozenType.First, ColorType.Black) },
        { 5,  new BetInfo(5,  DozenType.First, ColorType.Red) },
        { 6,  new BetInfo(6,  DozenType.First, ColorType.Black) },
        { 7,  new BetInfo(7,  DozenType.First, ColorType.Red) },
        { 8,  new BetInfo(8,  DozenType.First, ColorType.Black) },
        { 9,  new BetInfo(9,  DozenType.First, ColorType.Red) },
        { 10, new BetInfo(10, DozenType.First, ColorType.Black) },
        { 11, new BetInfo(11, DozenType.First, ColorType.Black) },
        { 12, new BetInfo(12, DozenType.First, ColorType.Red) },
        { 13, new BetInfo(13, DozenType.Second, ColorType.Black) },
        { 14, new BetInfo(14, DozenType.Second, ColorType.Red) },
        { 15, new BetInfo(15, DozenType.Second, ColorType.Black) },
        { 16, new BetInfo(16, DozenType.Second, ColorType.Red) },
        { 17, new BetInfo(17, DozenType.Second, ColorType.Black) },
        { 18, new BetInfo(18, DozenType.Second, ColorType.Red) },
        { 19, new BetInfo(19, DozenType.Second, ColorType.Red) },
        { 20, new BetInfo(20, DozenType.Second ,ColorType.Black) },
        { 21, new BetInfo(21, DozenType.Second, ColorType.Red) },
        { 22, new BetInfo(22, DozenType.Second, ColorType.Black) },
        { 23, new BetInfo(23, DozenType.Second, ColorType.Red) },
        { 24, new BetInfo(24, DozenType.Second, ColorType.Black) },
        { 25, new BetInfo(25, DozenType.Third, ColorType.Red) },
        { 26, new BetInfo(26, DozenType.Third, ColorType.Black) },
        { 27, new BetInfo(27, DozenType.Third, ColorType.Red) },
        { 28, new BetInfo(28, DozenType.Third, ColorType.Black) },
        { 29, new BetInfo(29, DozenType.Third, ColorType.Black) },
        { 30, new BetInfo(30, DozenType.Third, ColorType.Red) },
        { 31, new BetInfo(31, DozenType.Third, ColorType.Black) },
        { 32, new BetInfo(32, DozenType.Third, ColorType.Red) },
        { 33, new BetInfo(33, DozenType.Third, ColorType.Black) },
        { 34, new BetInfo(34, DozenType.Third, ColorType.Red) },
        { 35, new BetInfo(35, DozenType.Third, ColorType.Black) }, 
        { 36, new BetInfo(36, DozenType.Third, ColorType.Red) },
    };
    private int startSum = 200;
    private int maxBetsAtBoard = 1;

    public void SetPocketColor(int index, ColorType color)
    {
        if (pocketMapping.TryGetValue(index, out var pocket))
        {
            pocket.FieldColor = color;
            PocketColorChanged?.Invoke(index, color);
        }
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
    }

    public void RemoveFromStartSum(int amount)
    {
        startSum -= amount;
    }
}