using UnityEngine;

public class RangeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    private bool modifierApplied = false;

    public RangeBonusChip(BetCalculator betCalculator, BallResults ballResults, RangeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        minNumber = data.MinNumber;
        maxNumber = data.MaxNumber;
        bonusAmount = data.BonusAmount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
        modifierApplied = false;
    }

    public override void Revert()
    {
        if (modifierApplied)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, -bonusAmount);
            }
            modifierApplied = false;
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (modifierApplied) return;

        if (hit.FieldNumber >= minNumber && hit.FieldNumber <= maxNumber)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, bonusAmount);
            }
            modifierApplied = true;
        }
    }
}