using UnityEngine;
using System;
using Zenject;

public class ChipSpawnerManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int chipsToSpawn = 5;
    [SerializeField] private float spawnInterval = 0.5f;

    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private Transform spawnPoint;

    public Action StartSpawner;
    public Action ClearChips;

    private ChipSpawner chipSpawner;

    [Inject]
    public void Construct(ChipSpawner chipSpawner)
    {
        this.chipSpawner = chipSpawner;
    }

    private void Start()
    {
        chipSpawner.Configure(chipPrefab, spawnPoint, this);

        StartSpawner = () => chipSpawner.StartSpawing(chipsToSpawn, spawnInterval);
        ClearChips = () => chipSpawner?.ClearChips();
    }
}
