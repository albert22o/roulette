using UnityEngine;

[CreateAssetMenu(fileName = "XplusChipData", menuName = "Chips/XplusChipData")]

public class XplusChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new XplusChip(deps.BetCalculator, this);
    }
}
