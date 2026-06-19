using System;

public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        GoldenChips goldenChips,
        Money money)  
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteBallsManager, onBallSelected, goldenChips, money);
    }

    public Chip Create(ChipData data) => data.CreateChip(deps);
}