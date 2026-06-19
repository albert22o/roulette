using UnityEngine;

public class GoldenIngotBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly GoldenChips goldenChips;

    private bool modifierApplied = false;
    private int appliedAmount = 0;

    public GoldenIngotBonusChip(
        BetCalculator betCalculator,
        GoldenChips goldenChips,
        GoldenIngotBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.goldenChips = goldenChips;
    }

    public override void Apply()
    {
        appliedAmount = goldenChips.Amount;

        for (int i = 0; i <= 36; i++)
        {
            betCalculator.ModifyFieldNumberMultiplier(i, appliedAmount);
        }

        modifierApplied = true;
    }

    public override void Revert()
    {
        if (modifierApplied)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, -appliedAmount);
            }

            modifierApplied = false;
            appliedAmount = 0;
        }
    }
}