public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public RouletteBallsManager RouletteBallsManager { get; }  

    public ChipDependencies(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager)  
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteBallsManager = rouletteBallsManager;  
    }
}