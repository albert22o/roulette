using System;
using UnityEngine;

public class ChipSpawner
{
    public event Action<GameObject, Transform> ChipSpawnerRequested;
    public event Action ChipClearRecvested;

    public void RequestSpawn(GameObject prefab,Transform spawnPoint)
    {
        ChipSpawnerRequested?.Invoke(prefab, spawnPoint);
    }

    public void RecquestClear()
    {
        ChipClearRecvested?.Invoke();
    }
}