using System;

public class ChipDependencies
{
    public ChipHolder ChipHolder { get; set; }
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public IRouletteWheel RouletteWheel { get; }
    public BallResults BallResults { get; }
    public GoldenChips GoldenChips { get; }
    public Money Money { get; }
    public TableInfo TableInfo { get; }
    public RoundManager RoundManager { get; }
    public RewardCalculator RewardCalculator { get; }
    public LoseManager LoseManager { get; }

    public ChipDependencies(
        BetCalculator betCalculator,
        BetTable betTable,
        IRouletteWheel rouletteWheel,
        BallResults ballResult,
        GoldenChips goldenChips,
        TableInfo tableInfo,
        RoundManager roundManager,
        RewardCalculator rewardCalculator,
        LoseManager loseManager)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteWheel = rouletteWheel;  
        BallResults = ballResult;
        GoldenChips = goldenChips;
        TableInfo = tableInfo;
        RoundManager = roundManager;
        RewardCalculator = rewardCalculator;
        LoseManager = loseManager;
    }
}