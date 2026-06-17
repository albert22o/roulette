using System;
using UnityEngine;
using Zenject;

public class BaseMoneyVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private int initCost = 2;
    [SerializeField] private int nextCostDelta = 1;
    [SerializeField] private int purchasedMoney = 100;

    [SerializeField] private ButtonAnimation3D buttonAnimation;

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
        if (goldenChips.Amount >= Cost)
        {
            goldenChips.Remove(Cost);
            Cost += nextCostDelta;
            money.Add(purchasedMoney);
        }
    }

    public void Clicked()
    {
        TryBuy();
    }

    public void HoverEnter()
    {
        shop.ShowTooltip(titleKey, mainTextKey, tooltipPosition.position);
    }

    public void HoverExit()
    {
        shop.HIdeTooltip();
    }
}
