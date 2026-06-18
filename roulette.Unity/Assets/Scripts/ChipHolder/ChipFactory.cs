using System;

public class ChipFactory
{
    private readonly ChipDependencies deps;

    public ChipFactory(
        BetCalculator betCalculator,
        BetTable betTable,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected)  
    {
        deps = new ChipDependencies(betCalculator, betTable, rouletteBallsManager, onBallSelected);
    }

    public Chip Create(ChipData data) => data.CreateChip(deps);
}