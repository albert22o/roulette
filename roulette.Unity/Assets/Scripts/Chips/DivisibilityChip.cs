using UnityEngine;

public class DivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int divisor;
    private readonly int remainder;
    private readonly int bonusAmount;

    private bool modifyRequest = false;

    public DivisibilityChip(BetCalculator betCalculator, BallResults ballResults, DivisibilityChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        divisor = data.Divisor;
        remainder = data.Remainder;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

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
        if (hit?.FieldNumber % divisor == remainder)
            modifyRequest = true;
        else
            modifyRequest = false;
    }
}