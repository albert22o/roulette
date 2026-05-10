using UnityEngine;
using Zenject;

public class ShopInventoryItemInputHandler : MonoBehaviour, IMouseInteractable
{
    [SerializeField] private ShopInventoryItem inventoryItem;
    private bool isClicked;
    private Shop shop;

    public bool IsInteractable => inventoryItem != null;

    [Inject]
    public void Construct(Shop shop)
    {
        this.shop = shop;
    }

    public void OnMouseEnter()
    {
        shop.ShowDescription(inventoryItem.Item, inventoryItem.transform.position);
    }

    public void OnMouseExit()
    {
        shop.HideDescription();
        isClicked = false;
    }

    public void OnMouseDown()
    {
        isClicked = true;
    }

    public void OnMouseUp()
    {
        if (isClicked)
            shop.Sell(inventoryItem.Item);
        isClicked = false;
    }
}
