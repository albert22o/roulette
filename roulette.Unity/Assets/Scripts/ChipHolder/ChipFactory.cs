public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResults,
        GoldenChips goldenChips,
        TableInfo tableInfo)
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteWheel, ballResults, goldenChips, tableInfo);
    }

    public Chip Create(ChipData data, ChipHolder chipHolder) {
        deps.ChipHolder = chipHolder;
         return data.CreateChip(deps); 
    }
}