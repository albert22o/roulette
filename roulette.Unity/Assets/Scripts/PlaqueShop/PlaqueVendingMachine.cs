using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlaqueVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private OutlineController outline;
    [SerializeField] private SimpleTooltip tooltip;
    [SerializeField] private PlaqueShopView plaqueShopView;
    private PlaqueShop plaqueShop;
    private ShopFontConfig fontColorsConfig;
    public event Action Clicked;
    public bool IsInteractable => true;

    public void OnClick()
    {
        Clicked?.Invoke();
    }

    [Inject]
    private void Construct(PlaqueShop plaqueShop, ShopFontConfig fontColorsConfig)
    {
        this.plaqueShop = plaqueShop;
        this.fontColorsConfig = fontColorsConfig;
    }
    public void OnHoverEnter()
    {
        outline.ToggleMaterial(true);

        string title = "Магазин плаков";
        string description = "Купить 1 плак";

        var color = plaqueShop.CanBuy ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        description += $"\nЦена: <color={color}>{plaqueShop.Cost}</color>" + " босс фишка";

        tooltip.Show(title, description, transform.position);
    }

    public void OnHoverExit()
    {
        outline.ToggleMaterial(false);
        tooltip.Hide();
    }
}
