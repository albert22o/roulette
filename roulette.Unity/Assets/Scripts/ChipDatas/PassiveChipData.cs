using UnityEngine;

[CreateAssetMenu(fileName = "PassiveBonusChipData", menuName = "Chips/PassiveBonusChipData")]
public class PassiveChipData : ChipData
{
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PassiveChip(deps.BetCalculator, this);
    }
}