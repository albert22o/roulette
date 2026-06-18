using System;
using System.Linq;
using UnityEngine;

public class PrimeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int bonusAmount;

    private Action<HitInfo> onBallSelected;
    private int appliedFieldNumber = -1;
    public PrimeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        PrimeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
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
        foreach (var hit in hits.Where(h => IsPrime(h.FieldNumber)))
            AddToMemory(hit.BallHash);
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (IsInMemory(hit.BallHash))
        {
            betCalculator.ModifyFieldNumberMultiplier(hit.FieldNumber, bonusAmount);
            appliedFieldNumber = hit.FieldNumber;
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