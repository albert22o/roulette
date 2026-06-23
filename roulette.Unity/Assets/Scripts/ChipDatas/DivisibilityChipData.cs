using UnityEngine;

[CreateAssetMenu(fileName = "DivisibilityBonusChip", menuName = "Chips/DivisibilityBonusChip")]
public class DivisibilityChipData : ChipData
{
    public int Divisor;
    public int Remainder = 0;
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DivisibilityChip(deps.BetCalculator, deps.BallResults, this);
    }
}