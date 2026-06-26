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

    private int cost;
    private Shop shop;
    private BallInfoData boughtBall;
    private GoldenChips goldenChips;
    private BallHolder ballHolder;

    public bool IsInteractable => boughtBall == null;
    public event Action<BallInfoData> BallBought;
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
    public void Construct(Shop shop, GoldenChips goldenChips, BallHolder ballHolder)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
    }

    private void Start()
    {
        Cost = initCost;
    }

    public void TryBuy()
    {
        if (ballHolder.HasFreeSlot)
        {
            if (goldenChips.Amount >= Cost)
            {
                goldenChips.Remove(Cost);
                boughtBall = ballsTable.GetRandomItem() as BallInfoData;
                BallBought?.Invoke(boughtBall);
                shop.SetInteractionAvailable(false);
            }
        }
    }

    public void AddBallToInventory()
    {
        ballHolder.AddBall(boughtBall);
        boughtBall = null;
        shop.SetInteractionAvailable(true);
    }

    public void SellBall()
    {
        goldenChips.Add(shop.BallSellCost);
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
        shop.HideTooltip();
    }
}
