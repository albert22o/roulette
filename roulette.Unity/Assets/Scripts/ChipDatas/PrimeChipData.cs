using UnityEngine;

[CreateAssetMenu(fileName = "PrimeChip", menuName = "Chips/PrimeChip")]
public class PrimeChipData : ChipData
{
    public int BonusAmount = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PrimeChip(deps.BetCalculator, deps.BallResults, this);
    }
}