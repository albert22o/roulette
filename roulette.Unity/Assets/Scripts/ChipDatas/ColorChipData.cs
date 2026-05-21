using UnityEngine;

[CreateAssetMenu(fileName = "ColorChipData", menuName = "Chips/ColorChipData")]

public class ColorChipData : ChipData
{
    public string Color;
    public int ModifyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ColorChip(deps.BetCalculator, this);
    }
}
