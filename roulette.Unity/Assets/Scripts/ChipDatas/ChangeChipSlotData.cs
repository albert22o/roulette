using UnityEngine;

[CreateAssetMenu(fileName = "ChangeChipSlot", menuName = "Chips/ChangeChipSlot")]

public class ChangeChipSlotData : ChipData
{
    public int SlotsToRemove = 1;
    public int Multiplier = 2;
    public override Chip CreateChip(ChipDependencies deps)
    {
        return new ChangeChipSlot(deps.BetCalculator, deps.ChipHolder, this);
    }
}
