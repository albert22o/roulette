using System;
using System.Linq;
using UnityEngine;

public class PrimeBonusChip : Base_FieldBonusChip
{
    private readonly int bonusAmount;

    public PrimeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        PrimeBonusChipData data) : base(betCalculator, rouletteBallsManager, onBallSelected,  data)
    {
        bonusAmount = data.BonusAmount;
    }

    protected override bool CheckCondition(HitInfo hit) => IsPrime(hit.FieldNumber);

    protected override void ApplyModifier(HitInfo hit)
    {
        betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
    }

    protected override void RevertModifier()
    {
        betCalculator.ModifyFieldNumberMultiplier(appliedFieldNumber, -bonusAmount);
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