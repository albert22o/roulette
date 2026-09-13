using UnityEngine;

[CreateAssetMenu(fileName = "DozenChipData", menuName = "Chips/DozenChipData")]
public class DozenChipData : ChipData
{
    public DozenType DozenType;
    public int ModifyAmount;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DozenChip(deps.BetCalculator, this, deps.BallResults, deps.BetTable);
    }
}
