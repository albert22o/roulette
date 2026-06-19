using System;
public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public RouletteBallsManager RouletteBallsManager { get; }  
    public Action<HitInfo> OnBallSelected { get; }
    public GoldenChips GoldenChips { get; }
    public Money Money { get; }
    public ChipHolder ChipHolder { get; }
    public ChipDependencies(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        GoldenChips goldenChips,
        Money money,
        ChipHolder chipHolder)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteBallsManager = rouletteBallsManager;
        OnBallSelected = onBallSelected;
        GoldenChips = goldenChips;
        Money = money;
        ChipHolder = chipHolder;
    }
}