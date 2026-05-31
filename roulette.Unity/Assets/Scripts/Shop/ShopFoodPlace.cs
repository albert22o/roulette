using System;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class ShopFoodPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private ShopItemsRarityGroup foodTable;
    [SerializeField] private int cost = 10;
    [SerializeField] private ItemPlace itemPlace;


    private ItemData item;
    private Shop shop;
    private Inventory inventory;

    public bool IsInteractable => item != null;

    public event Action<bool> IsHoverChanged;
    public event Action<bool> HasItemChanged;


    [Inject] public void Construct(Shop shop, Inventory inventory)
    {
        this.shop = shop;
        this.inventory = inventory;
    }

    public void ShowDescription()
    {
        shop.ShowTooltip(item.NameKey, item.DescriptionKey, transform.position);
    }

    public void HideDescription()
    {
        shop.HIdeTooltip();
    }

    public void Clicked()
    {
        if (inventory.GetCurrency(CurrencyType.GoldenChips) >= cost)
        {
            inventory.RemoveCurrency(CurrencyType.GoldenChips, cost);
            Debug.Log($"Food {item.NameKey} bought");
            item = null;
            itemPlace.SetItem(null);
            HideDescription();
            HasItemChanged?.Invoke(false);
        }
    }

    public void HoverEnter()
    {
        ShowDescription();
        IsHoverChanged?.Invoke(true);
    }

    public void HoverExit()
    {
        HideDescription();
        IsHoverChanged?.Invoke(false);
    }

    public void UpdateItem()
    {
        item = foodTable.GetRandomItem();
        itemPlace.SetItem(item);
        HasItemChanged?.Invoke(true);
    }

}
