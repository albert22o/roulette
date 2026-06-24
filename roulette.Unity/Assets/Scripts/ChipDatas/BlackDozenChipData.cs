using UnityEngine;

[CreateAssetMenu(fileName = "BlackDozenChip", menuName = "Chips/BlackDozenChip")]
public class BlackDozenChipData : ChipData
{
    public int BonusAmount = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new BlackDozenChip(deps.BetCalculator, deps.BallResults, this);
    }
}