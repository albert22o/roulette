using System;
using UnityEngine;
using Zenject;

public class BallsVendingMachine : MonoBehaviour, IMouseInteractable, IClickable
{
    [SerializeField] private ShopItemsRarityGroup ballsTable;
    [SerializeField] private int initCost = 4;

    private Inventory inventory;
    private ItemFactory itemFactory;
    private int cost;

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
    public void Construct(Inventory inventory, ItemFactory factory)
    {
        this.inventory = inventory;
        itemFactory = factory;
    }

    private void Start()
    {
        Cost = initCost;
    }

    public void TryBuy()
    {
        if (inventory.Balls.Count < inventory.BallsCapacity)
        {
            if (inventory.GetCurrency(CurrencyType.GoldenChips) >= Cost)
            {
                inventory.RemoveCurrency(CurrencyType.GoldenChips, Cost);
                inventory.TryAddItem(itemFactory.Create(ballsTable.GetRandomItem()));
            }
        }
    }

    public void Clicked()
    {
        TryBuy();
    }
}
