using UnityEngine;

public class PassiveBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int bonusAmount;

    public PassiveBonusChip(BetCalculator betCalculator, PassiveBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.bonusAmount = data.BonusAmount;
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