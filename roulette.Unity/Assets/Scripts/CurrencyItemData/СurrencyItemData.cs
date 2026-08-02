using UnityEngine;

public abstract class СurrencyItemData : ItemData
{
    [SerializeField] private int amount = 1;
    public int Amount => amount;
}