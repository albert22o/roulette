using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private readonly List<Item> balls;
    private readonly List<Item> chips;

    public IReadOnlyList<Item> Balls => balls;
    public IReadOnlyList<Item> Chips => chips;

    public event Action ItemsChanged;

    public int BallsCapacity {  get; private set; }
    public int ChipsCapacity { get; private set; }


    public Inventory(InventoryConfig config)
    {
        balls = new List<Item>(config.BallsSlots);
        chips = new List<Item>(config.ChipsSlots);
        BallsCapacity = config.BallsSlots;
        ChipsCapacity = config.ChipsSlots;
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

    public bool CanAddItem(ItemData data)
    {
        switch (data.Type)
        {
            case ItemType.Ball:
                if (balls.Count < balls.Capacity)
                    return true;
                break;
            case ItemType.Chip:
                if (chips.Count < chips.Capacity)
                    return true;
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
}
