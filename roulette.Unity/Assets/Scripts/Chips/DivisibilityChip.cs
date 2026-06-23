using UnityEngine;

public class DivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int divisor;
    private readonly int remainder;
    private readonly int bonusAmount;

    private bool modifierApplied = false;

    public DivisibilityChip(BetCalculator betCalculator, BallResults ballResults, DivisibilityChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        divisor = data.Divisor;
        remainder = data.Remainder;
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

        if (hit.FieldNumber != 0 && hit.FieldNumber % divisor == remainder)
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, bonusAmount);
            }
            modifierApplied = true;
        }
    }
}