using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class BallsVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopItemsRarityGroup ballsTable;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;
    [SerializeField] private Transform tooltipPoint;

    [Space]
    [SerializeField] private OutlineController outline;

    private int cost;
    private Shop shop;
    private BallInfoData boughtBall;
    private GoldenChips goldenChips;
    private BallHolder ballHolder;
    private ShopFontConfig fontColorsConfig;
    private int spinsCount;

    public bool IsInteractable => boughtBall == null;
    [Inject] private readonly SignalBus signalBus;
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
    public void Construct(Shop shop, GoldenChips goldenChips, BallHolder ballHolder, ShopFontConfig fontColorsConfig)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.fontColorsConfig = fontColorsConfig;
    }

    private void Start()
    {
        Cost = shop.Config.BallCost;
    }

    public void ResetSpinAmount()
    {
        spinsCount = shop.Config.BallMachineUseAfterBoss;
    }

    public void TryBuy()
    {
        if (ballHolder.HasFreeSlot && spinsCount > 0)
        {
            if (goldenChips.AvailableAmount >= Cost)
            {
                goldenChips.Remove(Cost);
                boughtBall = ballsTable.GetRandomItem() as BallInfoData;
                BallBought?.Invoke(boughtBall);
                signalBus.Fire(new ItemPucharedSignal());
                shop.SetInteractionAvailable(false);
                spinsCount--;
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
        goldenChips.Add(shop.Config.BallSellCost);
        boughtBall = null;
        shop.SetInteractionAvailable(true);
    }

    public void OnClick()
    {
        TryBuy();
    }
       
    public void OnHoverEnter()
    {
        var text = mainTextKey;
        var color = goldenChips.Amount >= Cost ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        text += $"\nЦена: <color={color}>{Cost}</color>";
        color = spinsCount > 0 ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        text += $"\nОсталось прокруток: <color={color}>{spinsCount}</color>";
        shop.ShowTooltip(titleKey, text, tooltipPoint.position);
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
