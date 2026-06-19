using System;
using UnityEngine;

public class MultiplyDivisibilityBonusChip : Base_FieldBonusChip
{
    private readonly int divisor;
    private readonly int remainder;
    private readonly float multiplier;


    public MultiplyDivisibilityBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        MultiplyDivisibilityBonusChipData data) : base(betCalculator, rouletteBallsManager, onBallSelected, data)
    {
        remainder = data.Remainder;
        divisor = data.Divisor;
        multiplier = data.Multiplier;
    }

    protected override bool CheckCondition(HitInfo hit)
    {
        return hit.FieldNumber != 0 && hit.FieldNumber % divisor == remainder;
    }

    protected override void ApplyModifier(HitInfo hit)
    {
        betCalculator.MultiplyFieldNumberMultiplier(hit.FieldNumber, multiplier);
    }

    protected override void RevertModifier()
    {
        betCalculator.DivideFieldNumberMultiplier(appliedFieldNumber, multiplier);
    }
}