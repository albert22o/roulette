using UnityEngine;

public class ShopInventoryItemInputHandler : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopInventoryItem inventoryItem;
    public bool IsInteractable => inventoryItem != null;

    public void OnHoverEnter()
    {
        inventoryItem.ShowTooltip();
    }

    public void OnHoverExit()
    {
        inventoryItem.HideTooltip();
    }

    public void OnClick()
    {
        inventoryItem.Sell();
    }
}
