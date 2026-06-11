using System;
using System.Collections;
using UnityEngine;
using Zenject;
public class ChipSpawnerView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int chipsToSpawn = 5;
    [SerializeField] private float spawnInterval = 0.5f;

    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Transform spawnPoint;

    public Action StartSpawner;
    public Action ClearChips;

    private ChipSpawner chipSpawner;
    private Coroutine spawnCoroutine;

    [Inject]
    public void Construct(ChipSpawner chipSpawner)
    {
        this.chipSpawner = chipSpawner;
    }

    private void Start()
    {
        StartSpawner = () => StartSpawningWithInternal();
        ClearChips = () => ClearAllChips();
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null) 
            StopCoroutine(spawnCoroutine);
    }

    public void StartSpawningWithInternal()
    {
        if( spawnCoroutine != null )
            StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void ClearAllChips()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        chipSpawner.ClearChips();
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < chipsToSpawn; i++)
        {
            chipSpawner.SpawnChip(chipPrefab, spawnPoint);
            yield return new WaitForSeconds(spawnInterval);
        }
        spawnCoroutine = null;
    }
}
