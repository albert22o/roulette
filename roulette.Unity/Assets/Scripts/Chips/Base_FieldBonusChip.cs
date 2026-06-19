using System;
using UnityEngine;

public abstract class Base_FieldBonusChip : Chip
{
    protected readonly BetCalculator betCalculator;
    protected readonly RouletteBallsManager rouletteBallsManager;
    protected Action<HitInfo> onBallSelected;

    protected int appliedFieldNumber = -1;

    protected Base_FieldBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        ChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
    }

    public sealed override void Apply()
    {
        rouletteBallsManager.AllBallsStopped += OnAllBallsStopped;

        if (onBallSelected != null)
            onBallSelected += OnBallSelected;
    }

    public sealed override void Revert()
    {
        rouletteBallsManager.AllBallsStopped -= OnAllBallsStopped;

        if (onBallSelected != null)
            onBallSelected -= OnBallSelected;

        if (appliedFieldNumber != -1)
        {
            RevertModifier();
            appliedFieldNumber = -1;
        }
        ClearMemory();
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        ClearMemory();
        appliedFieldNumber = -1;

        foreach (var hit in hits)
            if (CheckCondition(hit))
                AddToMemory(hit.BallHash);
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (IsInMemory(hit.BallHash))
        {
            ApplyModifier(hit);
            appliedFieldNumber = hit.FieldNumber;
        }
    }

    protected abstract bool CheckCondition(HitInfo hit);

    protected abstract void ApplyModifier(HitInfo hit);

    protected abstract void RevertModifier();
}
