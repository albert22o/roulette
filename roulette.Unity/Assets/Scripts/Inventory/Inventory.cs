using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Inventory
{
    private readonly List<Item> balls;
    private readonly List<Item> chips;

    private ChipHolder chipHolder;

    public IReadOnlyList<Item> Balls => balls;
    public IReadOnlyList<Item> Chips => chips;

    public event Action ItemsChanged;

    public int BallsCapacity {  get; private set; }
    public int ChipsCapacity { get; private set; }

    [Inject]
    public void Construct(ChipHolder chipHolder)
    {
        this.chipHolder = chipHolder;
    }

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
                    chipHolder.AddChip(((ChipShopItemData)item.Data).ChipData);
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
                chipHolder.RemoveChip(((ChipShopItemData)item.Data).ChipData);
                break;
        }
        ItemsChanged?.Invoke();
    }
}
