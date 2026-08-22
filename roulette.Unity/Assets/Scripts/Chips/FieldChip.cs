using System;

public class FieldChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly int fieldNum;
    private readonly int modifyAmount;
    private HitInfo ballHit;

    public FieldChip(BetCalculator betCalculator, FieldChipData fieldChipData, BallResults ballResults) : base(fieldChipData) 
    {
        this.betCalculator = betCalculator;
        fieldNum = fieldChipData.FieldNum;
        modifyAmount = fieldChipData.ModifyAmount;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo info)
    {
        ballHit = info;
    }

    public override bool Apply()
    {
        betCalculator.ModifyFieldNumberMultiplier(fieldNum, modifyAmount);
        return ballHit != null && ballHit.FieldNumber == fieldNum;
    }

    public override void Revert()
    {
        betCalculator.ModifyFieldNumberMultiplier(fieldNum, -modifyAmount);
    }
}
