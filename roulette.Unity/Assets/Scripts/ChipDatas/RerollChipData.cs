using UnityEngine;

[CreateAssetMenu(fileName = "RerollChip", menuName = "Chips/RerollChip")]
public class RerollChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new RerollChip(deps.BetCalculator, deps.RoundManager, this);
    }
}
