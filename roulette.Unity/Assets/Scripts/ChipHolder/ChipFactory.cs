using System;

public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResults,
        GoldenChips goldenChips)
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteWheel, ballResults, goldenChips);
    }

    public Chip Create(ChipData data, ChipHolder chipHolder) {
        deps.ChipHolder = chipHolder;
         return data.CreateChip(deps); 
    }
}