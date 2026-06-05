using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChipSpawner 
{
    private GameObject chipPrefab;
    private Transform spawnPoint;

    private List<GameObject> spawnerChips = new();
    private Coroutine spawnCoroutine;

    private MonoBehaviour coroutineRunner;

    [Inject]
    public void Construct(MonoBehaviour coroutineRunner)
    {
        this.coroutineRunner = coroutineRunner;
       
    }
    public void Configure(GameObject chipPrefab, Transform spawnPoint)
    {
        this.chipPrefab = chipPrefab;
        this.spawnPoint = spawnPoint;
    }

    public void OnStartSpawing(int chipCount, float interval)
    {
        if (spawnCoroutine != null) 
            coroutineRunner.StopCoroutine(spawnCoroutine);

        spawnCoroutine = coroutineRunner.StartCoroutine(SpawnRoutine(chipCount, interval));
    }

    public void OnClearChips()
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
