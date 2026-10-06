using UnityEngine;

[CreateAssetMenu(fileName = "PassiveChipData", menuName = "Chips/PassiveChipData")]
public class PassiveChipData : ChipData
{
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PassiveChip(deps.BetCalculator, deps.BallResults, deps.BetTable, this);
    }
}