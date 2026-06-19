using System;
using UnityEngine;

public class DivisibilityBonusChip : Base_FieldBonusChip
{
  
    private readonly int divisor;
    private readonly int remainder;
    private readonly int bonusAmount;

    public DivisibilityBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        DivisibilityBonusChipData data): base(betCalculator, rouletteBallsManager, onBallSelected, data)
    {
        remainder = data.Remainder;
        divisor = data.Divisior;
        bonusAmount = data.BonusAmount;
    }

    

    protected override bool CheckCondition(HitInfo hit)
    {
        return hit.FieldNumber!=0 && hit.FieldNumber % divisor == remainder;
    }

    protected override void ApplyModifier(HitInfo hit)
    {
        betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
    }

    protected override void RevertModifier()
    {
        betCalculator.ModifyFieldNumberMultiplier(appliedFieldNumber, -bonusAmount);
    }
}
