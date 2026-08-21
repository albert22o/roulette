using UnityEngine;

public class GoldenIngotChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly GoldenChips goldenChips;

    public GoldenIngotChip(BetCalculator betCalculator, GoldenChips goldenChips, GoldenIngotChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.goldenChips = goldenChips;
    }

    public override bool Apply()
    {
        if (goldenChips.Amount == 0)
            return false;

        betCalculator.ModifyGeneralModifier(goldenChips.Amount);
        return true;
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-goldenChips.Amount);
    }
}