using System;
using TMPro;
using UnityEngine;

public class ShopPlace : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private CostView costView;

    private ItemData item;
    public ItemData Item
    {
        get => item;
        private set
        {
            item = value;
            ItemChanged?.Invoke(item);
        }
    }

    public event Action<ItemData> ItemChanged;


    public void SetItem(ItemData item)
    {
        Item = item;
        costView.SetCost(Item.Cost);
    }

    public void ShowDescription()
    {
        if (Item == null) return;
        shop.ShowDescription(Item, transform.position);
    }

    public void HideDescription()
    {
        shop.HideDescription();
    }

    public void Buy()
    {
        if (Item == null) return;
        if (shop.TryBuy(this))
        {
            Item = null;
            HideDescription();
            costView.SetCost(0);
        }
    }
}
