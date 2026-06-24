using UnityEngine;

[CreateAssetMenu(fileName = "RandomNumbersChip", menuName = "Chips/RandomNumbersChip")]
public class RandomNumbersChipData : ChipData
{
    public int BonusAmount = 4;
    public int NumbersCount = 4;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RandomNumbersChip(deps.BetCalculator, deps.BallResults, this);
    }
}