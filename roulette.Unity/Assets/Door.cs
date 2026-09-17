using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    private bool isOpened = false;
    private int isOpenedHash = Animator.StringToHash("IsOpened");
    private string tooltip;
    private string openedTooltip = "Закрыть дверь";
    private string closedTooltip = "Открыть дверь";
    public string Tooltip => tooltip;

    public bool CanInteract => true;

    public void Interact()
    {
        Toggle();
    }

    public void Toggle()
    {
        isOpened = !isOpened;
        animator.SetBool(isOpenedHash, isOpened);
        tooltip = isOpened ? openedTooltip : closedTooltip;
    }
}