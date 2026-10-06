using UnityEngine;

[CreateAssetMenu(fileName = "ChangeBallCountChip", menuName = "Chips/ChangeBallCountChip")]
public class ChangeBallCountChipData : ChipData
{
    public int ModifyAmount;
    public int BallSlotToRemove;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ChangeBallCountChip(deps.BallHolder, deps.BetCalculator, this,deps.BetTable, deps.BallResults);
    }
}
