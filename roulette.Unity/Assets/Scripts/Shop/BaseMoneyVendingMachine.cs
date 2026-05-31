using System;
using UnityEngine;
using Zenject;

public class BaseMoneyVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private int initCost = 2;
    [SerializeField] private int nextCostDelta = 1;
    [SerializeField] private int purchasedMoney = 100;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;
    [SerializeField] private Transform tooltipPosition;

    private Inventory inventory;
    private int cost;
    private Shop shop;


    public int Cost
    {
        get => cost; 
        private set
        {
            cost = value;
            CostChanged?.Invoke(cost);
        }
    }

    public bool IsInteractable => true;

    public event Action<int> CostChanged;

    [Inject]
    public void Construct(Inventory inventory, Shop shop)
    {
        this.inventory = inventory;
        this.shop= shop;
    }

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void TryBuy()
    {
        if (inventory.GetCurrency(CurrencyType.GoldenChips) >= Cost)
        {
            inventory.RemoveCurrency(CurrencyType.GoldenChips, Cost);
            Cost += nextCostDelta;
            inventory.AddCurrency(CurrencyType.BaseMoney, purchasedMoney);
        }
    }

    public void Clicked()
    {
        TryBuy();
    }

    public void HoverEnter()
    {
        shop.ShowTooltip(titleKey, mainTextKey, tooltipPosition.position);
    }

    public void HoverExit()
    {
        shop.HIdeTooltip();
    }
}
