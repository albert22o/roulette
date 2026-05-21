using UnityEngine;

[CreateAssetMenu(fileName = "EvenChipData", menuName = "Chips/EvenChipData")]

public class EvenChipData : ChipData
{
    public int ModifyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new EvenChip(deps.BetCalculator, this);
    }
}
