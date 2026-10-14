using UnityEngine;

[CreateAssetMenu(fileName = "D6ChipData", menuName = "Chips/D6ChipData")]

public class D6ChipData : ChipData
{
    public int MultiplyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new D6Chip(deps.BetCalculator, deps.BallResults, this, deps.BetTable);
    }
}
