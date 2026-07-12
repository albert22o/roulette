public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResults,
        GoldenChips goldenChips,
        TableInfo tableInfo,
        RoundManager roundManager,
        RewardCalculator rewardCalculator)
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteWheel, ballResults,
            goldenChips, tableInfo, roundManager, rewardCalculator);
    }

    public Chip Create(ChipData data, ChipHolder chipHolder) {
        deps.ChipHolder = chipHolder;
         return data.CreateChip(deps); 
    }
}