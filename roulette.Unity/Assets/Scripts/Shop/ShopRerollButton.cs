using System;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class ShopRerollButton : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private Shop shop;
    [SerializeField] private ShopChipPlacesController chipPlacesController;
    [SerializeField] private int initCost = 2;
    [SerializeField] private int costDelta;
    [SerializeField] private ButtonAnimation3D buttonAnimation;

    [SerializeField] private OutlineController outline;

    [Header("Tooltip")]
    [SerializeField] private LocalizedString title;
    [SerializeField] private LocalizedString mainText;

    private int cost;
    private GoldenChips goldenChips;
    private ShopFontConfig shopFontConfig;

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
    public void Construct(GoldenChips goldenChips, ShopFontConfig shopFontConfig)
    {
        this.goldenChips = goldenChips;
        this.shopFontConfig = shopFontConfig;
}

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void OnClick()
    {
        buttonAnimation.Press();
        if (goldenChips.AvailableAmount >= Cost)
        {
            goldenChips.Remove(Cost);
            Cost += costDelta;
            chipPlacesController.Reroll();
        }
    }

    public void OnHoverEnter()
    {
        var description = mainText.Resolve();
        var color = goldenChips.Amount >= cost ? shopFontConfig.CanBuyColorHex : shopFontConfig.CantBuyColorHex;
        var costText = Loc.Get("tooltip.price", color, cost);
        shop.ShowTooltip(title.Resolve(), description, costText, transform.position);
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
