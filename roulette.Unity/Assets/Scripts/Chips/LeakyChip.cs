using UnityEngine;

public class LeakyChip : ChipHolderDependentChip
{
    private readonly int bonusPerFreeSlot;

    public LeakyChip(BetCalculator betCalculator, LeakyChipData data) : base(betCalculator, data) { }

    protected override int CalculateBonus()
    {
        return 0;
    }
}