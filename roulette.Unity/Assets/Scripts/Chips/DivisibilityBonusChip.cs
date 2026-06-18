using System;
using UnityEngine;

public class DivisibilityBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int divisor;
    private readonly int remainder;
    private readonly int bonusAmount;

    private Action<HitInfo> onBallSelected;
    private int appliedFieldNumber = -1;
    public DivisibilityBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        DivisibilityBonusChipData data): base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
        remainder = data.Remainder;
        divisor = data.Divisior;
        bonusAmount = data.BonusAmount;
    }

    public override void Apply()
    {
        rouletteBallsManager.AllBallsStopped += OnAllBallsStopped;

        if (onBallSelected != null)
            onBallSelected += OnBallSelected;
    }

    public override void Revert()
    {
        rouletteBallsManager.AllBallsStopped -= OnAllBallsStopped;

        if (onBallSelected != null)
            onBallSelected -= OnBallSelected;

        if (appliedFieldNumber != -1)
        {
            betCalculator.ModifyFieldNumberMultiplier(appliedFieldNumber, -bonusAmount);
            appliedFieldNumber = -1;
        }
        ClearMemory();
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        ClearMemory();
        appliedFieldNumber = -1;
        foreach (var hit in hits)
            if (CheckDivisibility(hit))
                AddToMemory(hit.BallHash);
    }

    private void OnBallSelected (HitInfo hit)
    {
        if (IsInMemory(hit.BallHash))
        {
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
            appliedFieldNumber = hit.FieldNumber;
        }
    }

    private bool CheckDivisibility(HitInfo hit)
    {
        if (hit.FieldNumber == 0)
            return false;

        return hit.FieldNumber % divisor == remainder;
    }
}
