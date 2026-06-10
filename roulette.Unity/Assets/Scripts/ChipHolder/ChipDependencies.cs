public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public RouletteBallsManager RouletteBallsManager { get; }
    public RoundStateManager RoundStateManager { get; }
 
    public ChipDependencies(BetCalculator betCalculator, BetTable betTable, RouletteBallsManager rouletteBallsManager, RoundStateManager roundStateManager)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteBallsManager = rouletteBallsManager;
        RoundStateManager = roundStateManager;
    }
}
