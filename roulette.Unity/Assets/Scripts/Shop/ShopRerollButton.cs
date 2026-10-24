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

    [SerializeField] private OutlineController outline;

    [Header("Tooltip")]
    [SerializeField] private string titleKey;
    [SerializeField] private string mainTextKey;

    private int cost;
    private GoldenChips goldenChips;
    private ShopFontConfig shopFontConfig;
    private SFXManager sfxManager;
    public event Action RerollUsed;

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
    public void Construct(GoldenChips goldenChips, ShopFontConfig shopFontConfig, SFXManager sfxManager)
    {
        this.goldenChips = goldenChips;
        this.shopFontConfig = shopFontConfig;
        this.sfxManager = sfxManager;
}

    public void ResetCost()
    {
        Cost = initCost;
    }

    public void OnClick()
    {
        sfxManager.Play("ButtonClick");
        buttonAnimation.Press();
        if (goldenChips.AvailableAmount >= Cost)
        {
            goldenChips.Remove(Cost);
            Cost += costDelta;
            chipPlacesController.Reroll();
            RerollUsed?.Invoke();
        }
    }

    public void OnHoverEnter()
    {
        var description = $"{mainTextKey}";
        var color = goldenChips.Amount >= cost ? shopFontConfig.CanBuyColorHex : shopFontConfig.CantBuyColorHex;
        description += $"\nЦена: <color={color}>{cost}</color>";

        shop.ShowTooltip(titleKey, description, transform.position);
        outline.ToggleMaterial(true);
    }

    public void OnHoverExit()
    {
        shop.HideTooltip();
        outline.ToggleMaterial(false);
    }
}
