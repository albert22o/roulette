using UnityEngine;

public class DivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int divisor;
    private readonly int remainder;
    private readonly int bonusAmount;
    private readonly BetTable betTable;

    private bool modifyRequest = false;

    public DivisibilityChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, DivisibilityChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        divisor = data.Divisor;
        remainder = data.Remainder;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override float AppliedValue => bonusAmount;

    public override OperationType Operation => OperationType.Add;

    public override bool Apply()
    {
        if (!modifyRequest)
            return false;

        betCalculator.ModifyGeneralModifier(bonusAmount);
        return true;
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyGeneralModifier(-bonusAmount);
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