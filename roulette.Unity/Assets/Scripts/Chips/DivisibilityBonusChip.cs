using UnityEngine;

public class DivisibilityBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int divisor;
    private readonly int bonusAmount;

    public DivisibilityBonusChip(BetCalculator betCalculator, RouletteBallsManager rouletteBallsManager,  DivisibilityBonusChipData data): base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.divisor = data.Divisior;
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
        foreach (var hit in hits)
            if (CheckDivisibility(hit))
                betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber,bonusAmount);
    }

    private bool CheckDivisibility(HitInfo hit)
    {
        if (hit.FieldNumber == 0)
            return false;

        return hit.FieldNumber % divisor == 0;
    }
}
