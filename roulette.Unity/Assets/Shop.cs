using System;
using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopPlace[] shopPlaces;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private ShopItemsTable itemsTable;
    [SerializeField] private float tooltipHeight;
    [SerializeField] private float chipSellMultiplier = 0.5f;
    [SerializeField] private int ballSellCost = 3;

    private ShopItemsPool itemsPool = new();
    private Inventory inventory;


    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void Start()
    {
        itemsPool.SetTable(itemsTable);
        FillShopPlaces();
    }

    private void FillShopPlaces()
    {
        foreach (ShopPlace place in shopPlaces)
        {
            place.SetItem(itemsPool.GetRandomItem());
        }
    }

    public void Reroll()
    {

    }

    public void ShowDescription(Item item, Vector3 position)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetItem(item);
        tooltip.transform.position = position + Vector3.up * tooltipHeight;
    }

    public void ShowDescription(ItemData itemdata, Vector3 position)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetItemData(itemdata);
        tooltip.transform.position = position + Vector3.up * tooltipHeight;
    }

    public void HideDescription()
    {
        tooltip.gameObject.SetActive(false);
    }

    public bool TryBuy(ShopPlace place)
    {
        if (inventory.GetCurrency(CurrencyType.GoldenChips) >= place.Item.Cost)
        {
            string id = Guid.NewGuid().ToString();
            if (inventory.TryAddItem(new Item(place.Item, id)))
            {
                inventory.RemoveCurrency(CurrencyType.GoldenChips, place.Item.Cost);
                Debug.Log(place.Item.NameKey + " bought!");
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

    public void Sell(Item item)
    {
        if (item.Data.Type == ItemType.Ball)
        {
            if (inventory.Balls.Count > 1)
            {
                inventory.AddCurrency(CurrencyType.GoldenChips, ballSellCost);
                inventory.RemoveItem(item);
            }
        }
        else
        {
            inventory.AddCurrency(CurrencyType.GoldenChips, (int)(item.Data.Cost * chipSellMultiplier));
            inventory.RemoveItem(item);
        }
    }
}
