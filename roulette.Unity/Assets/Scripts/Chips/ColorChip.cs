using System;

public class ColorChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly ColorType color;
    private readonly int modifyAmount;

    private HitInfo ballHit;

    public override int AppliedMultiplier => modifyAmount;

    public ColorChip(BetCalculator betCalculator, ColorChipData colorChipData, BallResults ballResults) : base(colorChipData)
    {
        this.betCalculator = betCalculator;
        color = colorChipData.Color;
        modifyAmount = colorChipData.ModifyAmount;

        ballResults.BallSelected += OnBallSelected;

    }

    private void OnBallSelected(HitInfo info)
    {
        ballHit = info;
    }

    public override bool Apply()
    {
        betCalculator.ModifyColorMultiplier(color, modifyAmount);
        return ballHit != null && ballHit.FieldColor == color;
    }

    public override void Revert()
    {
        betCalculator.ModifyColorMultiplier(color, -modifyAmount);
    }
}
