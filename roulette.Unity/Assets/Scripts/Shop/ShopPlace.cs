using System;
using TMPro;
using UnityEngine;

public class ShopPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private Shop shop;
    [SerializeField] private CostView costView;
    [SerializeField] private ItemPlace itemPlace;

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

    public bool IsInteractable => Item != null;

    public event Action<ItemData> ItemChanged;


    public void SetItem(ItemData item)
    {
        Item = item;
        costView.SetCost(Item.Cost);
        itemPlace.SetItem(item);
    }

    public void ShowDescription()
    {
        if (Item == null) return;
        shop.ShowTooltip(Item.NameKey, Item.DescriptionKey, transform.position);
    }

    public void HideDescription()
    {
        shop.HIdeTooltip();
    }

    public void Buy()
    {
        if (Item == null) return;
        if (shop.TryBuy(this))
        {
            Item = null;
            itemPlace.SetItem(null);
            HideDescription();
            costView.SetCost(0);
        }
    }

    public void Clicked()
    {
        Buy();
    }

    public void HoverEnter()
    {
        ShowDescription();
    }

    public void HoverExit()
    {
        HideDescription();
    }
}
