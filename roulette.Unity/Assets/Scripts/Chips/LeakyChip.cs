using UnityEngine;

public class LeakyChip : ChipHolderDependentChip
{
    private readonly int bonusPerFreeSlot;
    public override OperationType Operation => OperationType.Add;
    public override float AppliedValue => Bonus;

    public LeakyChip(BetCalculator betCalculator, ChipHolder chipHolder, LeakyChipData data,
        BallResults ballResults, BetTable betTable) : base(betCalculator, chipHolder, data, ballResults, betTable) 
    {
        bonusPerFreeSlot = data.BonusPerFreeSlot;
    }


    protected override int CalculateBonus() => chipHolder.FreeSlotCount * bonusPerFreeSlot;
}