using UnityEngine;
using UnityEngine.Splines;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    private bool isOpened = false;
    private int isOpenedHash = Animator.StringToHash("IsOpened");
    public string Tooltip => Loc.Get(isOpened ? "interact.door_close" : "interact.door_open") + " 1 <sprite name=\"mask\">";

    public bool CanInteract => true;

    public void Interact()
    {
        //Toggle();
    }

    public void Toggle()
    {
        isOpened = !isOpened;
        animator.SetBool(isOpenedHash, isOpened);
    }
}