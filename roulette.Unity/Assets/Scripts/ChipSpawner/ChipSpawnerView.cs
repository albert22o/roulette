using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = System.Random;
public class ChipSpawnerView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spawnInterval = 0.5f;

    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float randomSpawnPositionMultiplier;

    private ChipSpawner chipSpawner;
    private List<GameObject> spawnedChips = new();
    private Coroutine spawnCoroutine;

    [Inject]
    public void Construct(ChipSpawner chipSpawner)
    {
        this.chipSpawner = chipSpawner;
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

        OnClearRequested();
    }

    private void SpawnChip()
    {
        Random randomX = new Random();
        Random randomZ = new Random();

        Vector3 offsetPos = new Vector3(
            (float)randomX.NextDouble() * randomSpawnPositionMultiplier,
            0,
            (float)randomZ.NextDouble()) * randomSpawnPositionMultiplier;

        GameObject chip = Instantiate(chipPrefab,spawnPoint.position + offsetPos, Quaternion.identity);
        spawnedChips.Add(chip);
    }

    private void OnClearRequested()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        foreach (var chip in spawnedChips)
        {
            if (chip != null)
                Destroy(chip);
        }
        spawnedChips.Clear();
    }

    private void StartSpawningWithInterval(int chipsToSpawn)
    {
        if (spawnCoroutine != null)
            StopCoroutine (spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine(chipsToSpawn));
    }

    private IEnumerator SpawnRoutine(int chipsToSpawn)
    {
        for (int i = 0; i < chipsToSpawn; i++)
        {
            SpawnChip();
            yield return new WaitForSeconds(spawnInterval);
        }
        spawnCoroutine = null;
    }
}
