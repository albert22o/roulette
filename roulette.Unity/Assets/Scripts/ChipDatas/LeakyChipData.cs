using UnityEngine;

[CreateAssetMenu(fileName = "LeakyChip", menuName = "Chips/LeakyChip")]
public class LeakyChipData : ChipData
{
    public int BonusPerFreeSlot = 2; 

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new LeakyChip(deps.BetCalculator, this);
    }
}