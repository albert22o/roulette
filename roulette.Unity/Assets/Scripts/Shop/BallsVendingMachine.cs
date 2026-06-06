using NUnit.Framework.Interfaces;
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
    private ItemFactory itemFactory = new();

    private int cost;
    private Shop shop;
    private Item boughtBall;
    private GoldenChips goldenChips;

    public bool IsInteractable => boughtBall == null;
    public event Action<ItemData> BallBought;
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
    public void Construct(Inventory inventory, Shop shop, GoldenChips goldenChips)
    {
        this.inventory = inventory;
        this.shop = shop;
        this.goldenChips = goldenChips;
    }

    private void Start()
    {
        Cost = initCost;
    }

    public void TryBuy()
    {
        if (inventory.Balls.Count < inventory.BallsCapacity)
        {
            if (goldenChips.Amount >= Cost)
            {
                goldenChips.Remove(Cost);
                var itemData = ballsTable.GetRandomItem();
                boughtBall = itemFactory.Create(itemData);
                BallBought?.Invoke(itemData);
                shop.SetInteractionAvailable(false);
            }
        }
    }

    public void AddBallToInventory()
    {
        inventory.TryAddItem(boughtBall);
        boughtBall = null;
        shop.SetInteractionAvailable(true);
    }

    public void SellBall()
    {
        shop.Sell(boughtBall);
        boughtBall = null;
        shop.SetInteractionAvailable(true);
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
