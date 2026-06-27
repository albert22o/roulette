using UnityEngine;

public class ShopInventoryItemInputHandler : MonoBehaviour, IMouseInteractable, IClickable, IHoverable
{
    [SerializeField] private ShopInventoryItem inventoryItem;
    public bool IsInteractable => inventoryItem != null;

    public void HoverEnter()
    {
        inventoryItem.ShowTooltip();
    }

    public void HoverExit()
    {
        inventoryItem.HideTooltip();
    }

    public void Clicked()
    {
        inventoryItem.Sell();
    }
}
