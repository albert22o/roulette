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

    public override void Apply()
    {
        betCalculator.ModifyGeneralModifier(bonusAmount);

    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-bonusAmount);
    }
}