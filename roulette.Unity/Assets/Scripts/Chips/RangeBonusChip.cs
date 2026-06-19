using System;
using System.Linq;
using UnityEngine;

public class RangeBonusChip : Base_FieldBonusChip
{
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    public RangeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        RangeBonusChipData data) : base(betCalculator, rouletteBallsManager, onBallSelected, data)
    {
        minNumber = data.MinNumber;
        maxNumber = data.MaxNumber;
        bonusAmount = data.BonusAmount;
    }

    protected override bool CheckCondition(HitInfo hit)
    {
        return hit.FieldNumber >= minNumber && hit.FieldNumber <= maxNumber;
    }

    protected override void ApplyModifier(HitInfo hit)
    {
        betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
    }

    protected override void RevertModifier()
    {
        betCalculator.ModifyFieldNumberMultiplier(appliedFieldNumber, - bonusAmount);
    }
}