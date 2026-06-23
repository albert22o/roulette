using UnityEngine;

public class PrimeChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int bonusAmount;

    private bool modifierApplied = false;

    public PrimeChip(BetCalculator betCalculator, BallResults ballResults, PrimeChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
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

        if (IsPrime(hit.FieldNumber))
        {
            for (int i = 0; i <= 36; i++)
            {
                betCalculator.ModifyFieldNumberMultiplier(i, bonusAmount);
            }
            modifierApplied = true;
        }
    }

    private bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (int i = 3; i <= Mathf.Sqrt(number); i += 2)
        {
            if (number % i == 0) return false;
        }

        return true;
    }
}