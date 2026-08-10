using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ChipDistributionCalculator
{
    private static readonly int[] Denominations = { 1000, 500, 100, 50, 25 };

    public static List<int> CalculateDistribution(int totalAmount, List<ChipThresholdData> thresholds)
    {
        Debug.Log($"[ChipDistribution] Начало расчета для суммы: {totalAmount}");

        List<int> chips = new List<int>();
        int remaining = totalAmount;

        (int mainDenom, int minChips) = GetThresholdParams(totalAmount, thresholds);
        Debug.Log($"[ChipDistribution] Выбран порог: основной номинал {mainDenom}, мин. фишек {minChips}");

        int maxMainByAmount = remaining / mainDenom;
        int mainCount = Mathf.Min(Mathf.FloorToInt(minChips * 0.6f), maxMainByAmount);

        if (mainCount < 2)
        {
            mainDenom = GetLowerDenomination(mainDenom);
            maxMainByAmount = remaining / mainDenom;
            mainCount = Mathf.Min(Mathf.FloorToInt(minChips * 0.6f), maxMainByAmount);
            Debug.Log($"[ChipDistribution] Основной номинал понижен до {mainDenom}, количество: {mainCount}");
        }

        int midCount = Mathf.FloorToInt(minChips * 0.3f);
        int smallCount = minChips - mainCount - midCount;

        int midDenom = GetNextDenomination(mainDenom);
        int smallDenom = GetNextDenomination(midDenom);

        for (int i = 0; i < mainCount; i++)
        {
            if (remaining >= mainDenom)
            {
                chips.Add(mainDenom);
                remaining -= mainDenom;
            }
        }

        for (int i = 0; i < midCount; i++)
        {
            if (remaining >= midDenom)
            {
                chips.Add(midDenom);
                remaining -= midDenom;
            }
        }

        for (int i = 0; i < smallCount; i++)
        {
            if (remaining >= smallDenom)
            {
                chips.Add(smallDenom);
                remaining -= smallDenom;
            }
        }

        foreach (int denom in Denominations)
        {
            while (remaining >= denom)
            {
                chips.Add(denom);
                remaining -= denom;
            }
        }

        Debug.Log($"[ChipDistribution] Распределение до размена ({chips.Count} фишек): {FormatChipList(chips)}, остаток: {remaining}");

        chips = ExchangeChips(chips, minChips);

        Debug.Log($"[ChipDistribution] Итоговое распределение ({chips.Count} фишек): {FormatChipList(chips)}");

        return chips;
    }

    private static string FormatChipList(List<int> chips)
    {
        var grouped = chips.GroupBy(x => x).OrderByDescending(x => x.Key);
        return string.Join(", ", grouped.Select(g => $"{g.Count()}x{g.Key}"));
    }

    private static (int mainDenom, int minChips) GetThresholdParams(int amount, List<ChipThresholdData> thresholds)
    {
        foreach (var threshold in thresholds)
        {
            if (amount < threshold.maxAmount)
            {
                return (threshold.mainDenomination, threshold.minChips);
            }
        }

        var last = thresholds[thresholds.Count - 1];
        return (last.mainDenomination, last.minChips);
    }

    private static List<int> ExchangeChips(List<int> chips, int targetMinChips)
    {
        if (chips.Count >= targetMinChips)
        {
            Debug.Log("[ChipDistribution] Размен не требуется: количество фишек достигает минимума.");
            return chips;
        }

        Debug.Log($"[ChipDistribution] Запуск размена. Текущее кол-во: {chips.Count}, цель: {targetMinChips}");

        chips.Sort((a, b) => b.CompareTo(a));

        int neededChips = targetMinChips - chips.Count;
        int exchangesMade = 0;

        for (int i = 0; i < chips.Count && exchangesMade < neededChips; i++)
        {
            int chipValue = chips[i];
            int chipIndex = System.Array.IndexOf(Denominations, chipValue);

            if (chipIndex >= Denominations.Length - 1)
                continue;

            int lowerDenom = Denominations[chipIndex + 1];
            int exchangeRate = chipValue / lowerDenom;

            if (exchangeRate > 1)
            {
                Debug.Log($"[ChipDistribution] Выполнен размен: 1x{chipValue} заменено на {exchangeRate}x{lowerDenom}");

                chips.RemoveAt(i);
                for (int j = 0; j < exchangeRate; j++)
                {
                    chips.Add(lowerDenom);
                }

                neededChips = targetMinChips - chips.Count;
                exchangesMade++;
                i = -1; 
                chips.Sort((a, b) => b.CompareTo(a));
            }
        }

        return chips;
    }

    private static int GetNextDenomination(int current)
    {
        int index = System.Array.IndexOf(Denominations, current);
        if (index >= 0 && index < Denominations.Length - 1)
            return Denominations[index + 1];
        return 25;
    }

    private static int GetLowerDenomination(int current)
    {
        int index = System.Array.IndexOf(Denominations, current);
        if (index > 0)
            return Denominations[index - 1];
        return 25;
    }
}