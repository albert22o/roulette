using System;

public class DozenChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BetTable betTable;
    private readonly DozenType dozenType;
    private readonly int modifyAmount;
    private HitInfo ballHit;
    private bool wasApplied;

    public override int AppliedMultiplier => modifyAmount;

    public DozenChip(BetCalculator betCalculator, DozenChipData dozenChipData, BallResults ballResults, BetTable betTable) : base(dozenChipData)
    {
        this.betCalculator = betCalculator;
        this.betTable = betTable;
        dozenType = dozenChipData.DozenType;
        modifyAmount = dozenChipData.ModifyAmount;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo info)
    {
        ballHit = info;
    }

    public override bool Apply()
    {
        bool dozenMatched = ballHit != null && ballHit.Dozen == dozenType;

        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (dozenMatched && betMatched)
        {
            betCalculator.ModifyDozenMultiplier(dozenType, modifyAmount);
            wasApplied = true;
            return true;
        }
        return false;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyDozenMultiplier(dozenType, -modifyAmount);
            wasApplied = false;
        }
    }
}