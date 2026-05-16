using UnityEngine;

public class VendingMachineInputHandler : MonoBehaviour, IMouseInteractable
{
    [SerializeField] private BaseMoneyVendingMachine vendingMachine;
    private bool isClicked;
    
    public bool IsInteractable => true;

    public void OnMouseEnter()
    {

    }

    public void OnMouseExit()
    {

        isClicked = false;
    }

    public void OnMouseDown()
    {
        isClicked = true;
    }

    public void OnMouseUp()
    {
        if (isClicked)
            vendingMachine.TryBuy();
        isClicked = false;
    }
}
