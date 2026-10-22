using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlaqueVendingMachine : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private OutlineController outline;
    [SerializeField] private ShopTooltip tooltip;
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

        string title = Loc.Get("shop.plaque_machine.title");
        string description = Loc.Get("shop.plaque_machine.desc");

        var color = plaqueShop.CanBuy ? fontColorsConfig.CanBuyColorHex : fontColorsConfig.CantBuyColorHex;
        description += Loc.Get("tooltip.price_boss_chips", color, plaqueShop.Cost);

        tooltip.Show(title, description, transform.position);
    }

    public void OnHoverExit()
    {
        outline.ToggleMaterial(false);
        tooltip.Hide();
    }
}
