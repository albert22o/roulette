using System;
using System.Collections.Generic;
using UnityEngine;

public class ChipSpawner
{
    public event Action<List<int>> ChipSpawnerRequested;
    public event Action ChipClearRequested;

    private List<ChipThresholdData> thresholds;

    private float mainChipRatio = 0.6f;
    private float midChipRatio = 0.3f;
    private int minimumMainChipsThreshold = 2;

    public void SetThresholds(List<ChipThresholdData> newThresholds)
    {
        thresholds = newThresholds;
    }

    public void SetDistributionSettings(float mainRatio, float midRatio, int minThreshold)
    {
        mainChipRatio = mainRatio;
        midChipRatio = midRatio;
        minimumMainChipsThreshold = minThreshold;
    }

    public void RequestSpawn(int totalAmount)
    {
        if (thresholds == null || thresholds.Count == 0)
        {
            Debug.LogError("[ChipSpawner] Thresholds не настроены!");
            return;
        }

        List<int> distribution = ChipDistributionCalculator.CalculateDistribution(
            totalAmount,
            thresholds,
            mainChipRatio,
            midChipRatio,
            minimumMainChipsThreshold
        );

        ChipSpawnerRequested?.Invoke(distribution);
    }

    public void RequestClear()
    {
        ChipClearRequested?.Invoke();
    }
}