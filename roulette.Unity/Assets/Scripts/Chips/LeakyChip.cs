using UnityEngine;

public class LeakyChip : ChipHolderDependentChip
{
    private readonly int bonusPerFreeSlot;

    public LeakyChip(BetCalculator betCalculator, ChipHolder chipHolder, LeakyChipData data) : base(betCalculator, chipHolder, data) 
    {
        bonusPerFreeSlot = data.BonusPerFreeSlot;
    }

    public override int AppliedMultiplier => Bonus;

    protected override int CalculateBonus() => chipHolder.FreeSlotCount * bonusPerFreeSlot;
}