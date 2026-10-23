using System;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class BaseMoneyVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private int initCost = 2;
    [SerializeField] private int nextCostDelta = 1;
    [SerializeField] private int purchasedMoney = 100;

    [SerializeField] private ButtonAnimation3D buttonAnimation;

    [SerializeField] private OutlineController outline;

    [Header("Tooltip")]
    [SerializeField] private LocalizedString title;
    [SerializeField] private LocalizedString mainText;
    [SerializeField] private Transform tooltipPosition;

    private int cost;
    private Shop shop;
    private GoldenChips goldenChips;
    private TableInfo tableInfo;
    private ShopFontConfig shopFontConfig;


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
    public void Construct(Shop shop, TableInfo tableInfo, GoldenChips goldenChips, ShopFontConfig shopFontConfig)
    {
        this.shop = shop;
        this.tableInfo = tableInfo;
        this.goldenChips = goldenChips;
        this.shopFontConfig = shopFontConfig;
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
        }
    }

    public void OnClick()
    {
        TryBuy();
    }

    public void OnHoverEnter()
    {
        ShowTooltip();
        outline.ToggleMaterial(true);
    }

    private void ShowTooltip()
    {
        var text = mainText.Resolve();
        var color = goldenChips.Amount >= Cost ? shopFontConfig.CanBuyColorHex : shopFontConfig.CantBuyColorHex;
        var cost = $"<color={color}>{Cost}</color>";
        shop.ShowTooltip(title.Resolve(), text, cost, tooltipPosition.position);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
