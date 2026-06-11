using UnityEngine;

[CreateAssetMenu(fileName = "XChipData",menuName = "Chips/XChipData")]
public class XChipData : ChipData
{
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new XChip(deps.BetCalculator,this);
    }
}
