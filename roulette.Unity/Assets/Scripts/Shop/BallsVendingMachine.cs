using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;
using Zenject.Asteroids;

public class BallsVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopItemsRarityGroup ballsTable;

    [Header("Tooltip")]
    [SerializeField] private LocalizedString title;
    [SerializeField] private LocalizedString mainText;
    [SerializeField] private Transform tooltipPoint;

    [Space]
    [SerializeField] private OutlineController outline;

    private int cost;
    private Shop shop;
    private BallInfoData boughtBall;
    private GoldenChips goldenChips;
    private BallHolder ballHolder;
    private ShopFontConfig fontColorsConfig;
    private SFXManager sfxManager;
    private int spinsCount;

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
    public void Construct(Shop shop, GoldenChips goldenChips, BallHolder ballHolder,
        ShopFontConfig fontColorsConfig, SFXManager sfxManager)
    {
        this.shop = shop;
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.fontColorsConfig = fontColorsConfig;
        this.sfxManager = sfxManager;
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
        var text = mainText.Resolve();
        var color = goldenChips.Amount >= Cost ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        text += Loc.Get("tooltip.spins_left", color, spinsCount);
        var cost = Loc.Get("tooltip.price", color, Cost);
        shop.ShowTooltip(title.Resolve(), text, cost, tooltipPoint.position);
        outline.ToggleMaterial(true);
    }

    public void PlayEjectBallSound()
    {
        sfxManager.Play("BallMachine");
    }

    public void PlayBallFellSound()
    {
        sfxManager.Play("BallMachineBallFell");
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
