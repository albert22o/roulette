using UnityEngine;

public class ShopStorageVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Shop shop;

    private int isOpenedHash = Animator.StringToHash("IsOpened");

    private void OnEnable()
    {
        shop.InteractionStarted += OnInteractionStarted;
        shop.InteractionFinished += OnInteractionFinished;
    }

    private void OnDisable()
    {
        shop.InteractionStarted -= OnInteractionStarted;
        shop.InteractionFinished -= OnInteractionFinished;
    }

    private void OnInteractionStarted()
    {
        animator.SetBool(isOpenedHash, true);
    }

    private void OnInteractionFinished()
    {
        animator.SetBool(isOpenedHash, false);
    }
}
