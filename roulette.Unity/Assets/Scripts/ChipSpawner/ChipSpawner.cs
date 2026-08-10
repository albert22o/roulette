using System;
using System.Collections.Generic;
using UnityEngine;

public class ChipSpawner
{
    public event Action<List<int>> ChipSpawnerRequested;
    public event Action ChipClearRequested;

    private List<ChipThresholdData> thresholds;

    public void SetThresholds(List<ChipThresholdData> newThresholds)
    {
        thresholds = newThresholds;
    }

    public void RequestSpawn(int totalAmount)
    {
        if (thresholds == null || thresholds.Count == 0)
        {
            Debug.LogError("[ChipSpawner] Thresholds не настроены!");
            return;
        }

        List<int> distribution = ChipDistributionCalculator.CalculateDistribution(totalAmount, thresholds);
        ChipSpawnerRequested?.Invoke(distribution);
    }

    public void RequestClear()
    {
        ChipClearRequested?.Invoke();
    }
}