using System;

public class ColorChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BetTable betTable;
    private readonly ColorType color;
    private readonly int modifyAmount;
    private bool wasApplied;

    private HitInfo ballHit;

    public override float AppliedValue => modifyAmount;
    public override OperationType Operation => OperationType.Add;

    public ColorChip(BetCalculator betCalculator, ColorChipData colorChipData, BallResults ballResults, BetTable betTable) : base(colorChipData)
    {
        this.betCalculator = betCalculator;
        this.betTable = betTable;
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
        bool colorMatched = ballHit != null && ballHit.FieldColor == color;

        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (colorMatched && betMatched)
        {
            betCalculator.ModifyColorMultiplier(color, modifyAmount);
            wasApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyColorMultiplier(color, -modifyAmount);
            wasApplied = false;
        }
    }
}