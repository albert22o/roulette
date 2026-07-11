using System;

public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public IRouletteWheel RouletteWheel { get; }
    public BallResults BallResults { get; }
    public GoldenChips GoldenChips { get; }
    public Money Money { get; }
    public ChipHolder ChipHolder { get; set; }
    public TableInfo TableInfo { get;  set; }

    public ChipDependencies(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResult,
        GoldenChips goldenChips,
        TableInfo tableInfo)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteWheel = rouletteWheel;  
        BallResults = ballResult;
        GoldenChips = goldenChips;
        TableInfo = tableInfo;
    }
}