using UnityEngine;

[CreateAssetMenu(fileName = "ColorChipData", menuName = "Chips/ColorChipData")]

public class ColorChipData : ChipData
{
    public ColorType Color;
    public int ModifyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ColorChip(deps.BetCalculator, this, deps.BallResults);
    }
}
