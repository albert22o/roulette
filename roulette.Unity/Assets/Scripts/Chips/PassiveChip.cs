using UnityEngine;

public class PassiveChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int bonusAmount;

    public PassiveChip(BetCalculator betCalculator, PassiveChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        bonusAmount = data.BonusAmount;
    }

    public override int AppliedMultiplier => bonusAmount;

    public override bool Apply()
    {
        betCalculator.ModifyGeneralModifier(bonusAmount);
        return true;
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-bonusAmount);
    }
}