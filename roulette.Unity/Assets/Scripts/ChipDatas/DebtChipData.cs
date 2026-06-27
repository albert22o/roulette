using UnityEngine;

[CreateAssetMenu(fileName = "DebtChip", menuName = "Chips/DebtChip")]
public class DebtChipData : ChipData
{
    public int DebtAmount = 2;

    public override Chip CreateChip(ChipDependencies deps)
    {
        return new DebtChip(deps.GoldenChips, this);
    }
}