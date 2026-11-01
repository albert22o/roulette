using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ChipDistributionCalculator
{
    [System.Serializable]
    public class DebugSettings
    {
        public bool enableGeneralLogs = false;
        public bool enableDetailedLogs = false;
    }

    private static DebugSettings debugSettings = new DebugSettings();

    public static void SetDebugMode(bool enableGeneral, bool enableDetailed)
    {
        debugSettings.enableGeneralLogs = enableGeneral;
        debugSettings.enableDetailedLogs = enableDetailed;
    }

    private static void LogGeneral(string message)
    {
        if (debugSettings.enableGeneralLogs)
            Debug.Log($"<color=white>[ChipDistribution]</color> {message}");
    }

    private static void LogDetail(string message)
    {
        if (debugSettings.enableDetailedLogs)
            Debug.Log($"<color=cyan>[ChipDistribution]</color> {message}");
    }

    private static void LogError(string message) => Debug.LogError($"<color=red>[ChipDistribution]</color> {message}");
    private static void LogWarning(string message) => Debug.LogWarning($"<color=orange>[ChipDistribution]</color> {message}");

    private static readonly int[] Denominations = { 1000000, 100000, 10000, 1000, 100 };

    public static List<int> CalculateDistribution(int totalAmount, List<ChipThresholdData> thresholds, float mainChipRatio, float midChipRatio, int minimumMainChipsThreshold)
    {
        LogDetail($"Начало расчета для суммы: {totalAmount}");

        List<int> chips = new List<int>();
        int remaining = totalAmount;

        (int mainDenom, int minChips) = GetThresholdParams(totalAmount, thresholds);
        LogDetail($"Выбран порог: основной номинал {mainDenom}, мин. фишек {minChips}");

        int maxMainByAmount = remaining / mainDenom;
        int mainCount = Mathf.Min(Mathf.FloorToInt(minChips * mainChipRatio), maxMainByAmount);

        if (mainCount < minimumMainChipsThreshold)
        {
            mainDenom = GetLowerDenomination(mainDenom);
            maxMainByAmount = remaining / mainDenom;
            mainCount = Mathf.Min(Mathf.FloorToInt(minChips * mainChipRatio), maxMainByAmount);
            LogDetail($"Основной номинал понижен до {mainDenom}, количество: {mainCount}");
        }

        int midCount = Mathf.FloorToInt(minChips * midChipRatio);
        int smallCount = minChips - mainCount - midCount;

        int midDenom = GetNextDenomination(mainDenom);
        int smallDenom = GetNextDenomination(midDenom);

        AddChipsByDenomination(chips, ref remaining, mainDenom, mainCount);
        AddChipsByDenomination(chips, ref remaining, midDenom, midCount);
        AddChipsByDenomination(chips, ref remaining, smallDenom, smallCount);

        foreach (int denom in Denominations)
        {
            while (remaining >= denom)
            {
                chips.Add(denom);
                remaining -= denom;
            }
        }

        LogDetail($"Распределение до размена ({chips.Count} фишек): {FormatChipList(chips)}, остаток: {remaining}");

        chips = ExchangeChips(chips, minChips);

        LogGeneral($"Итоговое распределение ({chips.Count} фишек): {FormatChipList(chips)}");

        return chips;
    }

    private static void AddChipsByDenomination(List<int> chips, ref int remaining, int denomination, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (remaining >= denomination)
            {
                chips.Add(denomination);
                remaining -= denomination;
            }
        }
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
            LogDetail("Размен не требуется: количество фишек достигает минимума.");
            return chips;
        }

        LogDetail($"Запуск размена. Текущее кол-во: {chips.Count}, цель: {targetMinChips}");

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
                LogDetail($"Выполнен размен: 1x{chipValue} заменено на {exchangeRate}x{lowerDenom}");

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

        if (index == -1)
        {
            LogError($"Номинал {current} не найден в массиве Denominations!");
            return GetSmallestDenomination();
        }

        if (index >= Denominations.Length - 1)
        {
            return Denominations[index];
        }

        return Denominations[index + 1];
    }

    private static int GetSmallestDenomination()
    {
        if (Denominations == null || Denominations.Length == 0)
        {
            LogError("Массив Denominations пуст или null!");
            return 1;
        }

        return Denominations[Denominations.Length - 1];
    }

    private static int GetLowerDenomination(int current)
    {
        int index = System.Array.IndexOf(Denominations, current);
        if (index > 0)
            return Denominations[index - 1];

        return GetSmallestDenomination();
    }
}