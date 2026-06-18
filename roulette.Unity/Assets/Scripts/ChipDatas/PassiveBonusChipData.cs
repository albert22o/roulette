using UnityEngine;

[CreateAssetMenu(fileName = "PassiveBonusChipData", menuName = "Chips/PassiveBonusChipData")]
public class PassiveBonusChipData : ChipData
{
    public int Remainder = 0;
    public int BonusAmount = 0;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new PassiveBonusChip(deps.BetCalculator, this);
    }
}