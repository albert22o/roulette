using UnityEngine;

public class ShopCellInputHandler : MonoBehaviour, IMouseInteractable
{
    [SerializeField] private ShopPlace shopPlace;
    private bool isClicked;

    public bool IsInteractable => shopPlace.Item != null;

    public void OnMouseEnter()
    {
        shopPlace.ShowDescription();
    }

    public void OnMouseExit()
    {
        shopPlace.HideDescription();
        isClicked = false;
    }

    public void OnMouseDown()
    {
        isClicked = true;
    }

    public void OnMouseUp()
    {
        if (isClicked)
            shopPlace.Buy();
        isClicked = false;
    }
}
