public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
 
    public ChipDependencies(BetCalculator betCalculator, BetTable betTable)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
    }
}
