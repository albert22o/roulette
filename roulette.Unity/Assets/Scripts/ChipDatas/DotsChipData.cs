using UnityEngine;

[CreateAssetMenu(fileName = "DotsChip", menuName = "Chips/DotsChip")]
public class DotsChipData : ChipData
{
    public int BonusPerChip = 2; 

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DotsChip(deps.BetCalculator, deps.ChipHolder, this);
    }
}