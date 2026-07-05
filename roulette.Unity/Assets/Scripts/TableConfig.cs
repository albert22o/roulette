using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TableConfig", menuName = "Configs/TableConfig")]
public class TableConfig : ScriptableObject
{
    [SerializeField] private int startSum = 200;
    [SerializeField] private int maxBetsAtBoard = 1;
    [SerializeField] private int betCount = 3;
    [SerializeField] private int rerollCount = 5;
    [SerializeField] private PocketData[] pockets = Array.Empty<PocketData>();

    public int StartSum => startSum;
    public int MaxBetsAtBoard => maxBetsAtBoard;
    public int BetCount => betCount;
    public int RerollCount => rerollCount;
    public PocketData[] Pockets => pockets;

#if UNITY_EDITOR
    [ContextMenu("Fill Default European Layout")]
    private void FillDefaultLayout()
    {
        pockets = new[]
        {
            new PocketData(0,  DozenType.Zero,   ColorType.Green),
            new PocketData(1,  DozenType.First,  ColorType.Red),
            new PocketData(2,  DozenType.First,  ColorType.Black),
            new PocketData(3,  DozenType.First,  ColorType.Red),
            new PocketData(4,  DozenType.First,  ColorType.Black),
            new PocketData(5,  DozenType.First,  ColorType.Red),
            new PocketData(6,  DozenType.First,  ColorType.Black),
            new PocketData(7,  DozenType.First,  ColorType.Red),
            new PocketData(8,  DozenType.First,  ColorType.Black),
            new PocketData(9,  DozenType.First,  ColorType.Red),
            new PocketData(10, DozenType.First,  ColorType.Black),
            new PocketData(11, DozenType.First,  ColorType.Black),
            new PocketData(12, DozenType.First,  ColorType.Red),
            new PocketData(13, DozenType.Second, ColorType.Black),
            new PocketData(14, DozenType.Second, ColorType.Red),
            new PocketData(15, DozenType.Second, ColorType.Black),
            new PocketData(16, DozenType.Second, ColorType.Red),
            new PocketData(17, DozenType.Second, ColorType.Black),
            new PocketData(18, DozenType.Second, ColorType.Red),
            new PocketData(19, DozenType.Second, ColorType.Red),
            new PocketData(20, DozenType.Second, ColorType.Black),
            new PocketData(21, DozenType.Second, ColorType.Red),
            new PocketData(22, DozenType.Second, ColorType.Black),
            new PocketData(23, DozenType.Second, ColorType.Red),
            new PocketData(24, DozenType.Second, ColorType.Black),
            new PocketData(25, DozenType.Third,  ColorType.Red),
            new PocketData(26, DozenType.Third,  ColorType.Black),
            new PocketData(27, DozenType.Third,  ColorType.Red),
            new PocketData(28, DozenType.Third,  ColorType.Black),
            new PocketData(29, DozenType.Third,  ColorType.Black),
            new PocketData(30, DozenType.Third,  ColorType.Red),
            new PocketData(31, DozenType.Third,  ColorType.Black),
            new PocketData(32, DozenType.Third,  ColorType.Red),
            new PocketData(33, DozenType.Third,  ColorType.Black),
            new PocketData(34, DozenType.Third,  ColorType.Red),
            new PocketData(35, DozenType.Third,  ColorType.Black),
            new PocketData(36, DozenType.Third,  ColorType.Red),
        };
    }
#endif
}

[Serializable]
public struct PocketData
{
    public int Number;
    public DozenType Dozen;
    public ColorType Color;

    public PocketData(int number, DozenType dozen, ColorType color)
    {
        Number = number;
        Dozen = dozen;
        Color = color;
    }
}