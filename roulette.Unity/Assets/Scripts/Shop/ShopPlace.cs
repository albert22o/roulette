using TMPro;
using UnityEngine;

public class ShopPlace : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private TextMeshPro costText;

    public ItemData Item { get; private set; }


    public void SetItem(ItemData item)
    {
        this.Item = item;
        costText.text = item.Cost.ToString();
    }

    public void ShowDescription()
    {
        if (Item == null) return;
        shop.ShowDescription(this);
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
            costText.text = "";
        }
    }
}
