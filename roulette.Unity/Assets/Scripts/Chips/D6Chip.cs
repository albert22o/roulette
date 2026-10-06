using System;

public class D6Chip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly D6ChipData d6ChipData;
    private readonly BetTable betTable;
    private bool wasApplied;

    public override float AppliedValue => d6ChipData.ModifyAmount;
    public override OperationType Operation => OperationType.Add;

    public D6Chip(BetCalculator betCalculator, BallResults ballResults,
        D6ChipData d6ChipData, BetTable betTable) : base(d6ChipData)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.d6ChipData = d6ChipData;
        this.ballResults.BallSelected += OnBallSelected;
        this.betTable = betTable;
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit))
        {
            wasApplied = true;
        }
    }

    public override void OnAddedToHolder()
    {
        base.OnAddedToHolder();
        ballResults.RandomBallSelection = true;
    }

    public override void OnRemovedFromHolder()
    {
        base.OnRemovedFromHolder();
        ballResults.RandomBallSelection = false;
    }

    public override bool Apply()
    {
        if (wasApplied)
            betCalculator.ModifyGeneralModifier(d6ChipData.ModifyAmount);
        return wasApplied;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyGeneralModifier(-d6ChipData.ModifyAmount);
            wasApplied = false;
        }
    }
}