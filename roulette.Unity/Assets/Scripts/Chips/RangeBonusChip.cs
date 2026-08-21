using UnityEngine;

public class RangeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    private bool modifyRequest = false;

    public RangeBonusChip(BetCalculator betCalculator, BallResults ballResults, RangeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
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
        if (hit?.FieldNumber >= minNumber && hit?.FieldNumber <= maxNumber)
            modifyRequest = true;
        else
            modifyRequest = false;
    }
}