using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = System.Random;

[Serializable]
public class ChipPrefabData
{
    public int denomination;
    public GameObject prefab;
}

public class ChipSpawnerView : MonoBehaviour
{
    [Header("Chip Prefabs")]
    [SerializeField] private List<ChipPrefabData> chipPrefabs = new List<ChipPrefabData>();

    [Header("Threshold Settings")]
    [SerializeField] private List<ChipThresholdData> thresholds = new List<ChipThresholdData>();

    [Header("Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float randomSpawnPositionMultiplier;

    [Header("Dynamics")]
    [SerializeField] private float startInterval = 0.4f;
    [SerializeField] private float minInterval = 0.08f;
    [SerializeField] private float intervalDecrement = 0.015f;
    [SerializeField] private int startBatchSize = 1;
    [SerializeField] private int maxBatchSize = 5;
    [SerializeField] private int iterationsPerBatchIncrease = 3;
    [SerializeField] private float viewingTime = 3f;
    [SerializeField] private float chipLifetime = 2f;

    private ChipSpawner chipSpawner;
    private WinningSequence winningSequence;
    private List<GameObject> spawnedChips = new List<GameObject>();
    private Coroutine spawnCoroutine;

    public List<ChipThresholdData> Thresholds => thresholds;

    [Inject]
    public void Construct(ChipSpawner chipSpawner, WinningSequence winningSequence)
    {
        this.chipSpawner = chipSpawner;
        this.winningSequence = winningSequence;
    }

    private void Start()
    {
        chipSpawner.SetThresholds(thresholds);
    }

    private void OnEnable()
    {
        chipSpawner.ChipSpawnerRequested += StartSpawningWithInterval;
        chipSpawner.ChipClearRequested += OnClearRequested;
    }

    private void OnDisable()
    {
        chipSpawner.ChipSpawnerRequested -= StartSpawningWithInterval;
        chipSpawner.ChipClearRequested -= OnClearRequested;
        ImmediateClear();
    }

    private GameObject GetChipPrefab(int denomination)
    {
        foreach (var data in chipPrefabs)
        {
            if (data.denomination == denomination)
                return data.prefab;
        }

        Debug.LogWarning($"[ChipSpawnerView] Префаб для номинала {denomination} не найден!");
        return null;
    }

    private void SpawnChip(int denomination)
    {
        Random randomX = new Random();
        Random randomZ = new Random();

        Vector3 offsetPos = new Vector3(
            (float)randomX.NextDouble() * randomSpawnPositionMultiplier,
            0,
            (float)randomZ.NextDouble() * randomSpawnPositionMultiplier);

        GameObject prefab = GetChipPrefab(denomination);
        if (prefab == null) return;

        GameObject chip = Instantiate(prefab, spawnPoint.position + offsetPos, prefab.transform.rotation);
        spawnedChips.Add(chip);
    }

    private void OnClearRequested()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(DelayedClear());
        }
        else
        {
            ImmediateClear();
        }
    }

    private void ImmediateClear()
    {
        foreach (var chip in spawnedChips)
        {
            if (chip != null)
                Destroy(chip);
        }
        spawnedChips.Clear();
    }

    private IEnumerator DelayedClear()
    {
        yield return new WaitForSeconds(chipLifetime);
        ImmediateClear();
    }

    private void StartSpawningWithInterval(List<int> chipDistribution)
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine(chipDistribution));
    }

    private IEnumerator SpawnRoutine(List<int> chipDistribution)
    {
        float currentInterval = startInterval;
        int currentBatchSize = startBatchSize;
        int chipsSpawned = 0;
        int iteration = 0;
        int totalChips = chipDistribution.Count;

        Debug.Log($"[ChipSpawnerView] Начинаем спаун {totalChips} фишек...");

        while (chipsSpawned < totalChips)
        {
            int chipsToSpawnNow = Mathf.Min(currentBatchSize, totalChips - chipsSpawned);

            for (int i = 0; i < chipsToSpawnNow; i++)
            {
                SpawnChip(chipDistribution[chipsSpawned]);
                chipsSpawned++;

                if (i < chipsToSpawnNow - 1)
                {
                    yield return new WaitForSeconds(0.02f);
                }
            }

            iteration++;
            if (iteration % iterationsPerBatchIncrease == 0 && currentBatchSize < maxBatchSize)
            {
                currentBatchSize++;
            }

            yield return new WaitForSeconds(currentInterval);
            currentInterval = Mathf.Max(minInterval, currentInterval - intervalDecrement);
        }

        Debug.Log($"[ChipSpawnerView] Все {totalChips} фишек заспаунены");
        yield return new WaitForSeconds(viewingTime);
        spawnCoroutine = null;
        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.Finish);
    }
}