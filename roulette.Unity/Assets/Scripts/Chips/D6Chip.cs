using System;

public class D6Chip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly D6ChipData d6ChipData;
    private readonly BetTable betTable;
    private HitInfo lastHit;

    public override int AppliedMultiplier => d6ChipData.ModifyAmount;

    public D6Chip(BetCalculator betCalculator, BallResults ballResults,
        D6ChipData d6ChipData, BetTable betTable) : base(d6ChipData)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.d6ChipData = d6ChipData;
        this.ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo hit)
    {
        lastHit = hit;
    }

    private bool IsFieldBetOn(int fieldNumber)
    {
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            if (amount > 0 && betInfo.FieldNumber == fieldNumber)
                return true;
        }
        return false;
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
        bool wasApplied = lastHit != null && IsFieldBetOn(lastHit.FieldNumber);

        if (wasApplied)
            betCalculator.ModifyGeneralModifier(d6ChipData.ModifyAmount);

        return wasApplied;
    }

    public override void Revert()
    {
        betCalculator.ModifyGeneralModifier(-d6ChipData.ModifyAmount);
    }
}