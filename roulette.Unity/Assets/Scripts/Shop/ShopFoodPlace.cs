using System;
using UnityEngine;
using Zenject;

public class ShopFoodPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private ShopItemsRarityGroup foodTable;
    [SerializeField] private int cost = 10;
    [SerializeField] private ItemPlace itemPlace;

    [SerializeField] private OutlineController outline;


    private FoodData item;
    private Shop shop;
    private GoldenChips goldenChips;
    private FoodHolder foodHolder;

    public bool IsInteractable => item != null;

    public event Action<bool> IsHoverChanged;
    public event Action<bool> HasItemChanged;


    [Inject] public void Construct(Shop shop, GoldenChips goldenChips, FoodHolder foodHolder)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
        this.foodHolder = foodHolder;
    }

    public void ShowDescription()
    {
        shop.ShowTooltip(item.Name, item.Description, transform.position);
    }

    public void HideDescription()
    {
        shop.HideTooltip();
    }

    public void OnClick()
    {
        if (goldenChips.AvailableAmount >= cost)
        {
            goldenChips.Remove(cost);
            foodHolder.AddFood(item);
            item = null;
            itemPlace.SetItem(null);
            HideDescription();
            HasItemChanged?.Invoke(false);
        }
    }

    public void OnHoverEnter()
    {
        ShowDescription();
        IsHoverChanged?.Invoke(true);
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        HideDescription();
        IsHoverChanged?.Invoke(false);
        outline.ToggleMaterial(false);
    }

    public void UpdateItem()
    {
        item = foodTable.GetRandomItem() as FoodData;
        itemPlace.SetItem(item);
        HasItemChanged?.Invoke(true);
    }

}
