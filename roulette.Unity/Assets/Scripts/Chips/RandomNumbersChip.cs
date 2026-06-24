using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomNumbersChip : Chip
{
    private readonly BetCalculator betCalculator;
    private readonly BallResults ballResults;
    private readonly int bonusAmount;
    private readonly int numbersCount;

    private bool modifyRequest = false;
    private List<int> selectedNumbers = new List<int>();

    public RandomNumbersChip(BetCalculator betCalculator, BallResults ballResults, RandomNumbersChipData data) : base(data)
    {
        this.betCalculator = betCalculator;
        this.ballResults = ballResults;
        bonusAmount = data.BonusAmount;
        numbersCount = data.NumbersCount;

        this.ballResults.BallSelected += OnBallSelected;
    }

    public override void Apply()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyGeneralModifier(bonusAmount);
        }
    }

    public override void Revert()
    {
        if (modifyRequest)
        {
            betCalculator.ModifyGeneralModifier(-bonusAmount);
        }
        selectedNumbers.Clear();
    }

    private void OnBallSelected(HitInfo hit)
    {
        if (selectedNumbers == null || selectedNumbers.Count == 0)
            selectedNumbers = GetRandomNumbers(numbersCount);

        if (selectedNumbers.Contains(hit.FieldNumber))
            modifyRequest = true;
        else
            modifyRequest = false;
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