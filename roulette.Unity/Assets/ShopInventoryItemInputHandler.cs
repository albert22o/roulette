using UnityEngine;
using Zenject;

public class ShopInventoryItemInputHandler : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopInventoryItem inventoryItem;
    private Shop shop;

    public bool IsInteractable => inventoryItem != null;

    [Inject]
    public void Construct(Shop shop)
    {
        this.shop = shop;
    }

    public void HoverEnter()
    {
        shop.ShowDescription(inventoryItem.Item, inventoryItem.transform.position);
    }

    public void HoverExit()
    {
        shop.HideDescription();
    }

    public void Clicked()
    {
        shop.Sell(inventoryItem.Item);
    }
}
