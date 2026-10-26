using System;
using UnityEngine;
using Zenject;

public class ShopFoodPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private ShopItemsRarityGroup foodTable;
    [SerializeField] private ItemPlace itemPlace;

    [SerializeField] private OutlineController outline;


    private FoodData item;
    private Shop shop;
    private GoldenChips goldenChips;
    private FoodHolder foodHolder;
    private ShopFontConfig shopFontConfig;
    [Inject] private readonly SignalBus signalBus;

    public bool IsInteractable => item != null;

    public event Action<bool> IsHoverChanged;
    public event Action<bool> HasItemChanged;
    public event Action FoodPurchased;


    [Inject] public void Construct(Shop shop, GoldenChips goldenChips, FoodHolder foodHolder, ShopFontConfig shopFontConfig)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
        this.foodHolder = foodHolder;
        this.shopFontConfig = shopFontConfig;
    }

    public void ShowDescription()
    {
        var description = $"{item.Description}";
        var color = goldenChips.Amount >= shop.Config.FoodCost ? shopFontConfig.CanBuyColorHex : shopFontConfig.CantBuyColorHex;
        var cost = $"<color={color}>{shop.Config.FoodCost}</color>";
        shop.ShowTooltip(item.Name, description, cost, transform.position);
    }

    public void HideDescription()
    {
        shop.HideTooltip();
    }

    public void OnClick()
    {
        if (goldenChips.AvailableAmount >= shop.Config.FoodCost)
        {
            goldenChips.Remove(shop.Config.FoodCost);
            foodHolder.AddFood(item);
            item = null;
            itemPlace.SetItem(null);
            HideDescription();
            HasItemChanged?.Invoke(false);
            FoodPurchased?.Invoke();
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
