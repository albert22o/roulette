using System;
using UnityEngine;
using Zenject;

public class BallsVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopItemsRarityGroup ballsTable;
    [SerializeField] private int initCost = 4;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;

    private Inventory inventory;
    private ItemFactory itemFactory;
    private int cost;
    private Shop shop;

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
    public void Construct(Inventory inventory, ItemFactory factory, Shop shop)
    {
        this.inventory = inventory;
        itemFactory = factory;
        this.shop = shop;
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

    public void HoverEnter()
    {
        shop.ShowTooltip(titleKey, mainTextKey, transform.position);
    }

    public void HoverExit()
    {
        shop.HIdeTooltip();
    }
}
