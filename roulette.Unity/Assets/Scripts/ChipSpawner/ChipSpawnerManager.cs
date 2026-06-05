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
        chipSpawner.Configure(chipPrefab, spawnPoint);

        StartSpawner = () => chipSpawner.OnStartSpawing(chipsToSpawn, spawnInterval);
        ClearChips = () => chipSpawner?.OnClearChips();
    }
    public void OnSpawnButtonClick()
    {
        Debug.Log("Кнопка Spawn нажата");
        StartSpawner?.Invoke();
    }

    public void OnClearButtonClick()
    {
        Debug.Log("Кнопка Clear нажата");
        ClearChips?.Invoke();
    }

    [ContextMenu("Text: Spawn Chip")]
    private void TestSpawn() => StartSpawner?.Invoke();

    [ContextMenu("Text: Clear Chips")]
    private void TestClesr() => ClearChips?.Invoke();
}
