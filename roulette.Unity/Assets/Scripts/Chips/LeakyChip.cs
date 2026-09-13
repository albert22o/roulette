using UnityEngine;

public class LeakyChip : ChipHolderDependentChip
{
    private readonly int bonusPerFreeSlot;

    public LeakyChip(BetCalculator betCalculator, ChipHolder chipHolder, LeakyChipData data,
        BallResults ballResults, BetTable betTable) : base(betCalculator, chipHolder, data, ballResults, betTable) 
    {
        bonusPerFreeSlot = data.BonusPerFreeSlot;
    }

    public override int AppliedMultiplier => Bonus;

    protected override int CalculateBonus() => chipHolder.FreeSlotCount * bonusPerFreeSlot;
}