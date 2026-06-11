using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class ChipSpawnerView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int chipsToSpawn = 5;
    [SerializeField] private float spawnInterval = 0.5f;

    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Transform spawnPoint;

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
        chipSpawner.ChipSpawnerRequested += OnChipSpawnRequested;
        chipSpawner.ChipClearRecvested += OnClearRequested;
    }

    private void OnDisable()
    {
        chipSpawner.ChipSpawnerRequested -= OnChipSpawnRequested;
        chipSpawner.ChipClearRecvested -= OnClearRequested;

        if ( spawnCoroutine != null) 
            StopCoroutine(spawnCoroutine);
    }

    private void OnChipSpawnRequested(GameObject prefab, Transform spawnPoint)
    {
        GameObject chip = Instantiate(prefab,spawnPoint.position, Quaternion.identity);
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

    public void StartSpawningWithInterval()
    {
        if (spawnCoroutine != null)
            StopCoroutine (spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < chipsToSpawn; i++)
        {
            chipSpawner.RequestSpawn(chipPrefab, spawnPoint);
            yield return new WaitForSeconds(spawnInterval);
        }
        spawnCoroutine = null;
    }
}
