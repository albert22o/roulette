using UnityEngine;

public class RangeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly BetTable betTable;
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    private bool modifyRequest = false;
    public override OperationType Operation => OperationType.Add;
    public override float AppliedValue => bonusAmount;

    public RangeBonusChip(BetCalculator betCalculator, BallResults ballResults, BetTable betTable, RangeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        this.betTable = betTable;
        minNumber = data.MinNumber;
        maxNumber = data.MaxNumber;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override bool Apply()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyGeneralModifier(bonusAmount);
            return true;
        }
        return false;
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
        bool rangeMatched = hit != null && hit.FieldNumber >= minNumber && hit.FieldNumber <= maxNumber;

        bool betMatched = hit != null && (
            BetTableExtentions.CheckColorMatch(betTable, hit) ||
            BetTableExtentions.CheckDozenMatch(betTable, hit) ||
            BetTableExtentions.CheckNumberMatch(betTable, hit));

        modifyRequest = rangeMatched && betMatched;
    }
}