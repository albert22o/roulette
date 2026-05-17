using System;
using UnityEngine;
using Zenject;

public class ShopRerollButton : MonoBehaviour, IMouseInteractable, IClickable
{
    [SerializeField] private Shop shop;
    [SerializeField] private int initCost = 2;
    [SerializeField] private int costDelta;

    private int cost;
    private Inventory inventory;

    public bool IsInteractable => true;
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

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void Clicked()
    {
        if (inventory.GetCurrency(CurrencyType.GoldenChips) >= Cost)
        {
            inventory.RemoveCurrency(CurrencyType.GoldenChips, Cost);
            Cost += costDelta;
            shop.Reroll();
        }
    }
}
