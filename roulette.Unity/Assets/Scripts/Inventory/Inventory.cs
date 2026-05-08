using System;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyType
{
    GoldenChips,
    BossChips,
    BaseMoney
}

public class Inventory
{
    private readonly List<Item> balls;
    private readonly List<Item> chips;

    private Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>
    {
        { CurrencyType.GoldenChips, 0 },
        { CurrencyType.BossChips, 0 },
        { CurrencyType.BaseMoney, 0 }
    };

    public event Action<CurrencyType, int> CurrencyChanged;
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

    public void SetCurrency(CurrencyType type, int count)
    {
        currencies[type] = count;
        CurrencyChanged?.Invoke(type, currencies[type]);
    }

    public void AddCurrency(CurrencyType type, int count)
    {
        currencies[type] += count;
        CurrencyChanged?.Invoke(type, currencies[type]);
    }

    public void RemoveCurrency(CurrencyType type, int count)
    {
        currencies[type] = Mathf.Max(0, currencies[type] - count);
        CurrencyChanged?.Invoke(type, currencies[type]);
    }

    public int GetCurrency(CurrencyType type) => currencies[type];
}
