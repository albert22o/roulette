public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(BetCalculator betCalculator, BetTable betTable)
    {
        deps = new ChipDependencies(betCalculator, betTable);
    }

    public Chip Create(ChipData data) => data.CreateChip(deps);
}
