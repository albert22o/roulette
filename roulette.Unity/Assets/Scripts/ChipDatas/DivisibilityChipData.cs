using UnityEngine;

[CreateAssetMenu(fileName = "DivisibilityChip", menuName = "Chips/DivisibilityChip")]
public class DivisibilityChipData : ChipData
{
    public int Divisor;
    public int Remainder = 0;
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DivisibilityChip(deps.BetCalculator, deps.BallResults, deps.BetTable, this);
    }
}