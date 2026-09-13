using UnityEngine;

public class MultiplyDivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly int divisor;
    private readonly int remainder;
    private readonly float multiplier;

    private bool modifyRequest = false;

    public MultiplyDivisibilityChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, MultiplyDivisibilityChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        divisor = data.Divisor;
        remainder = data.Remainder;
        multiplier = data.Multiplier;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override int AppliedMultiplier => (int)multiplier;

    public override bool Apply()
    {
        if (!modifyRequest)
            return false;

        betCalculator.MuliplyGeneralModifier(multiplier);
        return true;
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            betCalculator.DivideGeneralModifier(multiplier);
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        bool divisibilityMatched = hit != null && divisor != 0 && hit.FieldNumber % divisor == remainder;

        bool betMatched = hit != null && (
            BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit));

        modifyRequest = divisibilityMatched && betMatched;
    }
}