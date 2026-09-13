using UnityEngine;

[CreateAssetMenu(fileName = "FieldChipData", menuName = "Chips/FieldChipData")]
public class FieldChipData : ChipData
{
    public int FieldNum;
    public int ModifyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new FieldChip(deps.BetCalculator, this, deps.BallResults, deps.BetTable);
    }
}
