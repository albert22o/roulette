using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private readonly List<Item> balls;
    private readonly List<Item> chips;

    public int GoldenChipsCount { get; private set; }

    public event Action<int> GoldenChipsChanged;
    public event Action ItemsChanged;

    public Inventory(InventoryConfig config)
    {
        balls = new List<Item>(config.BallsSlots);
        chips = new List<Item>(config.ChipsSlots);
    }

    public bool TryAddItem(Item item)
    {
        switch (item.Data.Type)
        {
            case ItemType.Ball:
                if (balls.Count < balls.Capacity)
                {
                    balls.Add(item);
                    ItemsChanged?.Invoke();
                    return true;
                }
                else
                {
                    Debug.LogWarning("No more space for balls in inventory.");
                }
                break;
            case ItemType.Chip:
                if (chips.Count < chips.Capacity)
                {
                    chips.Add(item);
                    ItemsChanged?.Invoke();
                    return true;    
                }
                else
                {
                    Debug.LogWarning("No more space for chips in inventory.");
                }
                break;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        switch (item.Data.Type)
        {
            case ItemType.Ball:
                balls.Remove(item);
                break;
            case ItemType.Chip:
                chips.Remove(item);
                break;
        }
        ItemsChanged?.Invoke();
    }

    public void SetGoldenChips(int count)
    {
        GoldenChipsCount = count;
        GoldenChipsChanged?.Invoke(GoldenChipsCount);
    }

    public void AddGoldenChips(int count)
    {
        GoldenChipsCount += count;
        GoldenChipsChanged?.Invoke(GoldenChipsCount);
    }

    public void RemoveGoldenChips(int count)
    {
        GoldenChipsCount = Mathf.Max(0, GoldenChipsCount - count);
        GoldenChipsChanged?.Invoke(GoldenChipsCount);
    }
}