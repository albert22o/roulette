using System;
using System.Linq;
using UnityEngine;

public class RangeBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int minNumber;
    private readonly int maxNumber;
    private readonly int bonusAmount;

    private Action<HitInfo> onBallSelected;
    private int appliedFieldNumber = -1;
    public RangeBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        RangeBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
        minNumber = data.MinNumber;
        maxNumber = data.MaxNumber;
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
        foreach (var hit in hits.Where(CheckRange))
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

    private bool CheckRange(HitInfo hit)
    {
        return hit.FieldNumber >= minNumber && hit.FieldNumber <= maxNumber;
    }
}