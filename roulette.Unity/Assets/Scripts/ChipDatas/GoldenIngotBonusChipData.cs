using UnityEngine;

[CreateAssetMenu(fileName = "GoldenIngotBonusChip", menuName = "Chips/GoldenIngotBonusChip")]
public class GoldenIngotBonusChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new GoldenIngotBonusChip(deps.BetCalculator, deps.GoldenChips, this);
    }
}