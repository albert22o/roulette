using System;

public class FieldChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BetTable betTable;
    private readonly int fieldNum;
    private readonly int modifyAmount;
    private HitInfo ballHit;
    private bool wasApplied;

    public override float AppliedValue => modifyAmount;

    public override OperationType Operation => OperationType.Add;

    public FieldChip(BetCalculator betCalculator, FieldChipData fieldChipData, BallResults ballResults, BetTable betTable) : base(fieldChipData)
    {
        this.betCalculator = betCalculator;
        this.betTable = betTable;
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
        bool fieldMatched = ballHit != null && ballHit.FieldNumber == fieldNum;

        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (fieldMatched && betMatched)
        {
            betCalculator.ModifyFieldNumberMultiplier(fieldNum, modifyAmount);
            wasApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyFieldNumberMultiplier(fieldNum, -modifyAmount);
            wasApplied = false;
        }
    }
}