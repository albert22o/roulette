using System;
using UnityEngine;
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
    private Money money;


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
    public void Construct(Shop shop, Money money, GoldenChips goldenChips)
    {
        this.shop= shop;
        this.goldenChips = goldenChips;
        this.money = money;
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
            money.Add(purchasedMoney);
        }
    }

    public void OnClick()
    {
        TryBuy();
    }

    public void OnHoverEnter()
    {
        shop.ShowTooltip(titleKey, mainTextKey, tooltipPosition.position);
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
