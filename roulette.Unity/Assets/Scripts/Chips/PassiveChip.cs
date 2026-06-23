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
        for (int i = 0; i <= 36; i++)
        {
            betCalculator.ModifyFieldNumberMultiplier(i, bonusAmount);
        }

    }

    public override void Revert()
    {
        for (int i = 0; i <= 36; i++)
        {
            betCalculator.ModifyFieldNumberMultiplier(i, -bonusAmount);
        }
    }
}