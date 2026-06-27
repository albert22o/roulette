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

    public override void Apply()
    {
        betCalculator.ModifyGeneralModifier(goldenChips.Amount);
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-goldenChips.Amount);
    }
}