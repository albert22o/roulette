using System;

public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResults,
        GoldenChips goldenChips,
        Money money)
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteWheel, ballResults, goldenChips, money);
    }

    public Chip Create(ChipData data) => data.CreateChip(deps);
}