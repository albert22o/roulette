using UnityEngine;

public class GoldenIngotChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly GoldenChips goldenChips;
    private readonly BetTable betTable;
    private HitInfo ballHit;
    private int appliedAmount;
    private bool wasApplied;
    public override OperationType Operation => OperationType.Add;

    public GoldenIngotChip(BetCalculator betCalculator, GoldenChips goldenChips, BallResults ballResults, BetTable betTable, GoldenIngotChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.goldenChips = goldenChips;
        this.betTable = betTable;
        ballResults.BallSelected += OnBallSelected;
    }

    private void OnBallSelected(HitInfo info)
    {
        ballHit = info;
    }

    public override float AppliedValue => appliedAmount;

    public override bool Apply()
    {
        bool betMatched = ballHit != null && (
            BetTableExtentions.CheckColorMatch(betTable, ballHit) ||
            BetTableExtentions.CheckDozenMatch(betTable, ballHit) ||
            BetTableExtentions.CheckNumberMatch(betTable, ballHit));

        if (goldenChips.Amount == 0 || !betMatched)
            return false;

        appliedAmount = goldenChips.Amount;
        betCalculator.ModifyGeneralModifier(appliedAmount);
        wasApplied = true;
        return true;
    }

    public override void Revert()
    {
        if (wasApplied)
        {
            betCalculator.ModifyGeneralModifier(-appliedAmount);
            wasApplied = false;
        }
    }
}