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
    private GoldenChips goldenChips;

    public bool IsInteractable => item != null;

    public event Action<bool> IsHoverChanged;
    public event Action<bool> HasItemChanged;


    [Inject] public void Construct(Shop shop, Inventory inventory, GoldenChips goldenChips)
    {
        this.shop = shop;
        this.inventory = inventory;
        this.goldenChips = goldenChips;
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
        if (goldenChips.Amount >= cost)
        {
            goldenChips.Remove(cost);
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
