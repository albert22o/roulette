using System.Linq;
using UnityEngine;

public class PrimeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int bonusAmount;

    public PrimeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        PrimeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.bonusAmount = data.BonusAmount;
    }

    public override void Apply()
    {
        rouletteBallsManager.AllBallsStopped += OnAllBallsStopped;
    }

    public override void Revert()
    {
        rouletteBallsManager.AllBallsStopped -= OnAllBallsStopped;
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        foreach (var hit in hits.Where(h => IsPrime(h.FieldNumber)))
        {
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
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