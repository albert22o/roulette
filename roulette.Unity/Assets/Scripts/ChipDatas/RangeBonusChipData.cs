using UnityEngine;

[CreateAssetMenu(fileName = "RangeBonusChip", menuName = "Chips/RangeBonusChip")]
public class RangeBonusChipData : ChipData
{
    public int MinNumber = 19;
    public int MaxNumber = 36;
    public int BonusAmount = 3;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RangeBonusChip(deps.BetCalculator, deps.BallResults, deps.BetTable, this);
    }
}