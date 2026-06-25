using System;
using UnityEngine;

public class ChipSpawner
{
    public event Action<int> ChipSpawnerRequested;
    public event Action ChipClearRequested;

    public void RequestSpawn(int amount)
    {
        ChipSpawnerRequested?.Invoke(amount);
    }

    public void RequestClear()
    {
        ChipClearRequested?.Invoke();
    }
}