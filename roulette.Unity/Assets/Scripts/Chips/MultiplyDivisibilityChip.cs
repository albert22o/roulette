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
        if(modifyRequest)
        {
            for(int i = 0; i<=36; i++) 
                betCalculator.MultiplyFieldNumberMultiplier(i, multiplier);
        }
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            for (int i = 0; i <= 36; i++)
                betCalculator.DivideFieldNumberMultiplier(i, multiplier);
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (hit?.FieldNumber != 0 && hit?.FieldNumber % divisor == remainder)
            modifyRequest = true;
        else
            modifyRequest = false;
    }
}