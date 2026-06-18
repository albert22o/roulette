using System;
using UnityEngine;

public class MultiplyDivisibilityBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int divisor;
    private readonly int remainder;
    private readonly float multiplier;

    private Action<HitInfo> onBallSelected;
    private int appliedFieldNumber = -1;

    public MultiplyDivisibilityBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        MultiplyDivisibilityBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
        remainder = data.Remainder;
        divisor = data.Divisor;
        multiplier = data.Multiplier;
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
            betCalculator.DivideFieldNumberMultiplier(appliedFieldNumber, multiplier);
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

    private void OnBallSelected(HitInfo hit)
    {
        if (IsInMemory(hit.BallHash))
        {
            betCalculator.MultiplyFieldNumberMultiplier(hit.FieldNumber, multiplier);
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