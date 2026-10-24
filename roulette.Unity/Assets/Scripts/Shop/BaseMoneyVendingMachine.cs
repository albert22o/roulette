using System;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

public class BaseMoneyVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private int initCost = 2;
    [SerializeField] private int nextCostDelta = 1;
    [SerializeField] private int purchasedMoney = 100;

    [SerializeField] private ButtonAnimation3D buttonAnimation;

    [SerializeField] private OutlineController outline;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;
    [SerializeField] private Transform tooltipPosition;

    private int cost;
    private Shop shop;
    private GoldenChips goldenChips;
    private TableInfo tableInfo;
    private ShopFontConfig shopFontConfig;
    private SFXManager sfxManager;
    public event Action BaseMoneyPurchased;


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
    public void Construct(Shop shop, TableInfo tableInfo, GoldenChips goldenChips, ShopFontConfig shopFontConfig, SFXManager sfxManager)
    {
        this.shop = shop;
        this.tableInfo = tableInfo;
        this.goldenChips = goldenChips;
        this.shopFontConfig = shopFontConfig;
        this.sfxManager = sfxManager;
    }

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void TryBuy()
    {
        buttonAnimation.Press();
        if (goldenChips.AvailableAmount >= Cost)
        {
            goldenChips.Remove(Cost);
            Cost += nextCostDelta;
            tableInfo.AddToStartSum(purchasedMoney);
            ShowTooltip();
            BaseMoneyPurchased?.Invoke();
        }
    }

    public void OnClick()
    {
        sfxManager.Play("ButtonClick");
        TryBuy();
    }

    public void OnHoverEnter()
    {
        ShowTooltip();
        outline.ToggleMaterial(true);
    }

    private void ShowTooltip()
    {
        var text = mainTextKey;
        var color = goldenChips.Amount >= Cost ? shopFontConfig.CanBuyColorHex : shopFontConfig.CantBuyColorHex;
        text += $"\nЦена: <color={color}>{Cost}</color>";
        shop.ShowTooltip(titleKey, text, tooltipPosition.position);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
