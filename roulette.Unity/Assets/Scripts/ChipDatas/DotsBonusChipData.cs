using UnityEngine;

[CreateAssetMenu(fileName = "DotsBonusChip", menuName = "Chips/DotsBonusChip")]
public class DotsBonusChipData : ChipData
{
    public int BonusPerChip = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DotsBonusChip(deps.BetCalculator, deps.ChipHolder, this);
    }
}