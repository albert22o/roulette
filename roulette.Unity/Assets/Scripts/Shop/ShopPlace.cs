using System;
using UnityEngine;
using Zenject;

public class ShopPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private Shop shop;
    [SerializeField] private DrumNumberDisplay costView;

    private ItemData item;
    private Inventory inventory;
    private readonly ItemFactory itemFactory = new();
    private Item createdItem;
    private GoldenChips goldenChips;
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
    public bool IsAnimationPlaying { get; private set; } = false;

    public event Action<ItemData> ItemChanged;

    [Inject]
    public void Construct(Inventory inventory, GoldenChips goldenChips)
    {
        this.inventory = inventory;
        this.goldenChips = goldenChips;
    }

    public void SetItem(ItemData item)
    {
        Item = item;
        costView.SetNumber(Item.Cost);
        ItemChanged?.Invoke(item);
        IsAnimationPlaying = true;
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
        if (TryBuy(this))
        {
            Item = null;
            ItemChanged?.Invoke(Item);
            HideDescription();
            costView.SetNumber(0);
            IsAnimationPlaying = true;
        }
    }

    private bool TryBuy(ShopPlace place)
    {
        if (goldenChips.Amount >= place.Item.Cost)
        {
            if (inventory.CanAddItem(place.Item))
            {
                createdItem = itemFactory.Create(place.Item);
                goldenChips.Remove(place.Item.Cost);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log(place.Item.NameKey + " not enough money!");
            return false;
        }
    }

    public void UpdateInventory()
    {
        if (createdItem == null) return;
        inventory.TryAddItem(createdItem);
        createdItem = null;
    }

    public void OnRaised()
    {
        IsAnimationPlaying = false;
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
