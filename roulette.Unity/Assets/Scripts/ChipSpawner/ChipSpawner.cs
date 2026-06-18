using System;
using UnityEngine;

public class ChipSpawner
{
    public event Action ChipSpawnerRequested;
    public event Action ChipClearRequested;

    public void RequestSpawn()
    {
        ChipSpawnerRequested?.Invoke();
    }

    public void RecquestClear()
    {
        ChipClearRequested?.Invoke();
    }
}