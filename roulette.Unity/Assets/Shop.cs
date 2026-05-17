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

    [SerializeField] private ShopRerollButton rerollButton;
    [SerializeField] private BaseMoneyVendingMachine baseMoneyVendingMachine;
    [SerializeField] private BallsVendingMachine ballsVendingMachine;
    [SerializeField] private ShopFoodPlace foodPlace;

    private ShopItemsPool itemsPool = new();
    private Inventory inventory;
    private ItemFactory itemFactory;


    [Inject]
    public void Construct(Inventory inventory, ItemFactory factory)
    {
        this.inventory = inventory;
        itemFactory = factory;
    }

    private void Start()
    {
        itemsPool.SetTable(itemsTable);
        OnQuotaUpdated();
    }

    public void OnQuotaUpdated()
    {
        FillShopPlaces();
        rerollButton.ResetCost();
        baseMoneyVendingMachine.ResetCost();
        foodPlace.UpdateItem();
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
        foreach (ShopPlace place in shopPlaces)
        {
            if (place.Item != null)
            {
                place.SetItem(itemsPool.GetRandomItem());
            }
        }
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

            if (inventory.CanAddItem(place.Item))
            {
                inventory.TryAddItem(itemFactory.Create(place.Item));
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
