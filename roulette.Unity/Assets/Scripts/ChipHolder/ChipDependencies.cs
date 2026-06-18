using System;
public class ChipDependencies
{
    public BetCalculator BetCalculator { get; }
    public BetTable BetTable { get; }
    public RouletteBallsManager RouletteBallsManager { get; }  
    public Action<HitInfo> OnBallSelected { get; }
    public ChipDependencies(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected)
    {
        BetCalculator = betCalculator;
        BetTable = betTable;
        RouletteBallsManager = rouletteBallsManager;
        OnBallSelected = onBallSelected;
    }
}