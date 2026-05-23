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
        }
    }

    public void HoverEnter()
    {
        ShowDescription();
    }

    public void HoverExit()
    {
        HideDescription();
    }

    public void UpdateItem()
    {
        item = foodTable.GetRandomItem();
        itemPlace.SetItem(item);
    }

}
