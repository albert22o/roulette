using UnityEngine;

[CreateAssetMenu(fileName = "MultiplyDivisibilityChip", menuName = "Chips/MultiplyDivisibilityBonusChip")]
public class MultiplyDivisibilityChipData : ChipData
{
    public int Divisor;
    public int Remainder = 0;
    public float Multiplier = 2f;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new MultiplyDivisibilityChip(deps.BetCalculator, deps.BallResults, this);
    }
}