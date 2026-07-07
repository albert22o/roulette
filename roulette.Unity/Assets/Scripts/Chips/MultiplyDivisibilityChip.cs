using UnityEngine;

public class MultiplyDivisibilityChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int divisor;
    private readonly int remainder;
    private readonly float multiplier;

    private bool modifyRequest = false;

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
        if (modifyRequest)
        {
            betCalculator.MuliplyGeneralModifier(multiplier);
        }
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
        if (hit?.FieldNumber % divisor == remainder)
            modifyRequest = true;
        else
            modifyRequest = false;
    }
}