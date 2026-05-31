using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

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

    public bool CanReroll => CheckRerollAvailable();


    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
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

    public void ShowTooltip(string title, string mainText, Vector3 position)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetData(title, mainText);
        tooltip.transform.position = position + Vector3.up * tooltipHeight;
    }

    public void HIdeTooltip()
    {
        tooltip.gameObject.SetActive(false);
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

    public bool CheckRerollAvailable()
    {
        bool hasItem = false;
        foreach(ShopPlace place in shopPlaces)
        {
            if (place.IsAnimationPlaying) return false;
            if (place.Item != null) hasItem = true;
        }
        if (hasItem == false) return false;
        return true;
    }
}
