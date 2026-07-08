using UnityEngine;

[CreateAssetMenu(fileName = "LostChipData", menuName = "Chips/LostChipData")]

public class LostChipData : ChipData
{
    public int SlotsToRemove = 1;
    public int Multiplier = 2;
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new LostChip(deps.BetCalculator, deps.ChipHolder, this);
    }
}
