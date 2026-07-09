using System;
using UnityEngine;
using Zenject;

public class ShopFoodPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private ShopItemsRarityGroup foodTable;
    [SerializeField] private int cost = 10;
    [SerializeField] private ItemPlace itemPlace;


    private ItemData item;
    private Shop shop;
    private GoldenChips goldenChips;

    public bool IsInteractable => item != null;

    public event Action<bool> IsHoverChanged;
    public event Action<bool> HasItemChanged;


    [Inject] public void Construct(Shop shop, GoldenChips goldenChips)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
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
        if (goldenChips.Amount >= cost)
        {
            goldenChips.Remove(cost);
            Debug.Log($"Food {item.Name} bought");
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
    }

    public void OnHoverExit()
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
