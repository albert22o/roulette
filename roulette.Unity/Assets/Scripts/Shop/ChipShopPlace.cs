using System;
using UnityEngine;
using Zenject;

public class ChipShopPlace : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private Shop shop;
    [SerializeField] private DrumNumberDisplay costView;
    [SerializeField] private OutlineController outline;

    private ChipData item;
    private ChipHolder chipHolder;
    private GoldenChips goldenChips;
    private ChipData boughtItem;
    private RarityData rarityData;
    private ShopFontConfig fontColorsConfig;

    
    public ChipData Item
    {
        get => item;
        private set
        {
            item = value;
            ItemChanged?.Invoke(item);
        }
    }

    public bool IsInteractable => Item != null && IsAnimationPlaying == false;
    public bool IsAnimationPlaying { get; private set; } = false;

    public event Action<ItemData> ItemChanged;
    public event Action ItemBought;

    [Inject]
    public void Construct(ChipHolder chipHolder, GoldenChips goldenChips, RarityData rarityData, ShopFontConfig fontColorsConfig)
    {
        this.chipHolder = chipHolder;
        this.goldenChips = goldenChips;
        this.rarityData = rarityData;
        this.fontColorsConfig = fontColorsConfig;
    }

    public void SetItem(ChipData item)
    {
        Item = item;
        costView.SetNumber(Item.Cost);
        IsAnimationPlaying = true;
    }

    public void ShowDescription()
    {
        if (Item == null) return;
        string description = Item.Description;
        if (Item.RarityType != RarityType.None)
        {
            var rarity = rarityData.GetRarityDisplayData(Item.RarityType);
            description += "\nРедкость: <color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(rarity.Color) + ">" + rarity.Name + "</color>";
        }
        var color = goldenChips.Amount >= Item.Cost ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        description += $"\nЦена: <color={color}>{Item.Cost}</color>";
        shop.ShowTooltip(Item.Name, description, transform.position);
    }

    public void HideDescription()
    {
        shop.HideTooltip();
    }

    public void Buy()
    {
        if (Item == null) return;
        if (TryBuy(this))
        {
            HideDescription();
            costView.SetNumber(0);
            IsAnimationPlaying = true;
            ItemBought?.Invoke();
        }
    }

    private bool TryBuy(ChipShopPlace place)
    {
        if (goldenChips.AvailableAmount >= place.Item.Cost)
        {
            if (chipHolder.HasFreeSlot)
            {
                goldenChips.Remove(place.Item.Cost);
                boughtItem = Item;
                Item = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log(place.Item.Name + " not enough money!");
            return false;
        }
    }

    public void UpdateInventory()
    {
        if (boughtItem != null)
        {
            chipHolder.AddChip(boughtItem);
            boughtItem = null;
        }
    }

    public void OnRaised()
    {
        IsAnimationPlaying = false;
    }

    public void OnClick()
    {
        Buy();
    }

    public void OnHoverEnter()
    {
        ShowDescription();
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        HideDescription();
        outline.ToggleMaterial(false);
    }
}
