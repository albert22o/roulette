using UnityEngine;

public class MultiplyDivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int divisor;
    private readonly int remainder;
    private readonly float multiplier;

    private bool modifierApplied = false;

    public MultiplyDivisibilityChip(BetCalculator betCalculator, BallResults ballResults, MultiplyDivisibilityChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        divisor = data.Divisor;
        remainder = data.Remainder;
        multiplier = data.Multiplier;

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
                betCalculator.DivideFieldNumberMultiplier(i, multiplier);
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
                betCalculator.MultiplyFieldNumberMultiplier(i, multiplier);
            }
            modifierApplied = true;
        }
    }
}