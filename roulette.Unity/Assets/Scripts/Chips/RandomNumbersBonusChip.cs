using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomNumbersBonusChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly RouletteBallsManager rouletteBallsManager;
    private readonly int bonusAmount;
    private readonly int numbersCount;

    private Action<HitInfo> onBallSelected;
    private List<int> appliedNumbers = new List<int>();
    private bool modifierApplied = false;

    public RandomNumbersBonusChip(
        BetCalculator betCalculator,
        RouletteBallsManager rouletteBallsManager,
        Action<HitInfo> onBallSelected,
        RandomNumbersBonusChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.rouletteBallsManager = rouletteBallsManager;
        this.onBallSelected = onBallSelected;
        bonusAmount = data.BonusAmount;
        numbersCount = data.NumbersCount;
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
            foreach (int number in appliedNumbers)
            {
                betCalculator.ModifyFieldNumberMultiplier(number, -bonusAmount);
            }

            appliedNumbers.Clear();
            modifierApplied = false;
        }

        ClearMemory();
    }

    private void OnAllBallsStopped(HitInfo[] hits)
    {
        ClearMemory();
        appliedNumbers.Clear();
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
            appliedNumbers = GetRandomNumbers(numbersCount);

            foreach (int number in appliedNumbers)
            {
                betCalculator.ModifyFieldNumberMultiplier(number, bonusAmount);
            }

            modifierApplied = true;
        }
    }

    private List<int> GetRandomNumbers(int count)
    {
        var availableNumbers = Enumerable.Range(1, 36).ToList();
        var selectedNumbers = new List<int>();

        for (int i = 0; i < count && availableNumbers.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableNumbers.Count);
            selectedNumbers.Add(availableNumbers[randomIndex]);
            availableNumbers.RemoveAt(randomIndex);
        }

        return selectedNumbers;
    }
}