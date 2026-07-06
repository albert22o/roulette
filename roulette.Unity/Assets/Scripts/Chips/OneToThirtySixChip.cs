using UnityEngine;

public class OneToThirtySixChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;

    private bool modifyRequest = false;
    private readonly int[] pseudoZeroNumbers = { 1, 2, 35, 36 };

    public OneToThirtySixChip(BetCalculator betCalculator, BallResults ballResults, OneToThirtySixChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
        if (modifyRequest)
        {
            int zeroMultiplier = betCalculator.ZeroMultiplier;
            int currentMultiplier = betCalculator.NumMultiplier;
            int delta = zeroMultiplier - currentMultiplier;

            foreach (int number in pseudoZeroNumbers)
            {
                betCalculator.ModifyFieldNumberMultiplier(number, delta);
            }
        }
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            int zeroMultiplier = betCalculator.ZeroMultiplier;
            int currentMultiplier = betCalculator.NumMultiplier;
            int delta = zeroMultiplier - currentMultiplier;

            foreach (int number in pseudoZeroNumbers)
            {
                betCalculator.ModifyFieldNumberMultiplier(number, -delta);
            }
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        foreach (int number in pseudoZeroNumbers)
        {
            if (hit?.FieldNumber == number)
            {
                modifyRequest = true;
                return;
            }
        }

        modifyRequest = false;
    }
}