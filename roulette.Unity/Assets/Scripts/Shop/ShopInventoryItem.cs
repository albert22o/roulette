using UnityEngine;

public class ShopInventoryItem : MonoBehaviour
{
    [SerializeField] private ItemPlace place;

    private Shop shop;
    public ItemData ItemData { get; private set;  }
    public int Index { get; set; }

    public void SetItem(ItemData item)
    {
        ItemData = item;
        place.SetItem(item);
    }

    public void SetShop(Shop shop)
    {
        this.shop = shop;
    }

    public void ShowTooltip()
    {
        if (ItemData == null) return;
        shop.ShowTooltip(ItemData.Name, ItemData.Description, transform.position);
    }

    public void HideTooltip()
    {
        if (ItemData == null) return;
        shop.HideTooltip();
    }

    public void Sell()
    {
        if (ItemData == null) return;
        if (shop.TrySell(ItemData, Index))
        {
            shop.HideTooltip();
        }
            
    }
}
