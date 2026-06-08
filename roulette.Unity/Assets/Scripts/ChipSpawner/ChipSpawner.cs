using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChipSpawner 
{
    private GameObject chipPrefab;
    private Transform spawnPoint;
    private MonoBehaviour coroutineRunner;

    private List<GameObject> spawnerChips = new();
    private Coroutine spawnCoroutine;

    public void Configure(GameObject chipPrefab, Transform spawnPoint, MonoBehaviour coroutineRunner)
    {
        this.chipPrefab = chipPrefab;
        this.spawnPoint = spawnPoint;
        this.coroutineRunner = coroutineRunner;

    }

    public void StartSpawing(int chipCount, float interval)
    {
        if (spawnCoroutine != null) 
            coroutineRunner.StopCoroutine(spawnCoroutine);

        spawnCoroutine = coroutineRunner.StartCoroutine(SpawnRoutine(chipCount, interval));
    }

    public void ClearChips()
    {
        if(spawnCoroutine != null)
        {
            coroutineRunner.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        foreach (var chip in spawnerChips)
        {
            if (chip != null)
                UnityEngine.Object.Destroy(chip);
        }

        spawnerChips.Clear();
    }

    private IEnumerator SpawnRoutine (int chipCount, float interval)
    {
        for (int i = 0; i < chipCount; i++)
        {
            GameObject chip = UnityEngine.Object.Instantiate(chipPrefab,spawnPoint.position, Quaternion.identity);
            spawnerChips.Add(chip);
            yield return new WaitForSeconds(interval);
        }
        spawnCoroutine = null;
    }

}
