public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager)  
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteBallsManager);
    }

    public Chip Create(ChipData data) => data.CreateChip(deps);
}