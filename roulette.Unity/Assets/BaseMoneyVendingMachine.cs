using System;
using UnityEngine;
using Zenject;

public class BaseMoneyVendingMachine : MonoBehaviour
{
    [SerializeField] private int initCost = 2;
    [SerializeField] private int nextCostDelta = 1;
    [SerializeField] private int purchasedMoney = 100;

    private Inventory inventory;
    private int cost;


    public int Cost
    {
        get => cost; 
        private set
        {
            cost = value;
            CostChanged?.Invoke(cost);
        }
    }
    public event Action<int> CostChanged;

    [Inject]
    public void Construct(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void Awake()
    {
        ResetCost();
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

}
