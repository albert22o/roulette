using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackDozenBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int bonusAmount;

    private Action<HitInfo> onBallSelected;
    private DozenType appliedDozen;
    private bool modifierApplied = false;

    public BlackDozenBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        BlackDozenBonusChipData data) : base(data)
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

        if (modifierApplied)
        {
            foreach (int number in GetDozenNumbers(appliedDozen))
            {
                betCalculator.ModifyFieldNumberMultiplier(number, -bonusAmount);
            }

            modifierApplied = false;
        }

        ClearMemory();
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        ClearMemory();
        modifierApplied = false;

        foreach (var hit in hits)
        {
            AddToMemory(hit.BallHash);
        }
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (IsInMemory(hit.BallHash) && !modifierApplied)
        {
            appliedDozen = GetRandomDozen();

            foreach (int number in GetDozenNumbers(appliedDozen))
            {
                betCalculator.ModifyFieldNumberMultiplier(number, bonusAmount);
            }

            modifierApplied = true;
        }
    }

    private DozenType GetRandomDozen()
    {
        int random = UnityEngine.Random.Range(0, 3);
        return random switch
        {
            0 => DozenType.First,
            1 => DozenType.Second,
            2 => DozenType.Third,
            _ => DozenType.First
        };
    }

    private List<int> GetDozenNumbers(DozenType dozen)
    {
        var numbers = new List<int>();

        switch (dozen)
        {
            case DozenType.First:
                for (int i = 1; i <= 12; i++) numbers.Add(i);
                break;
            case DozenType.Second:
                for (int i = 13; i <= 24; i++) numbers.Add(i);
                break;
            case DozenType.Third:
                for (int i = 25; i <= 36; i++) numbers.Add(i);
                break;
        }

        return numbers;
    }
}