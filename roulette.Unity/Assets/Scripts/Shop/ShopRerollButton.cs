using System;
using UnityEngine;
using Zenject;

public class ShopRerollButton : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private Shop shop;
    [SerializeField] private ShopChipPlacesController chipPlacesController;
    [SerializeField] private int initCost = 2;
    [SerializeField] private int costDelta;
    [SerializeField] private ButtonAnimation3D buttonAnimation;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;

    private int cost;
    private Money money;
    private GoldenChips goldenChips;

    public bool IsInteractable => chipPlacesController.CheckRerollAvailable();
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
    public void Construct(Money money, GoldenChips goldenChips)
    {
        this.goldenChips = goldenChips;
        this.money = money;
    }

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void Clicked()
    {
        buttonAnimation.Press();
        if (goldenChips.Amount >= Cost)
        {
            goldenChips.Remove(Cost);
            Cost += costDelta;
            chipPlacesController.Reroll();
        }
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
