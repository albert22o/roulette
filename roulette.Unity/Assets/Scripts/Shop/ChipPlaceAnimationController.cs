using UnityEngine;

public class ChipPlaceAnimationController : MonoBehaviour
{
    [SerializeField] private ItemPlace itemPlace;
    [SerializeField] private ShopPlace shopPlace;
    [SerializeField] private Animator animator;

    private ItemData item;
    private int isRaisedHash = Animator.StringToHash("IsRaised");

    private void OnEnable()
    {
        shopPlace.ItemChanged += OnItemChanged;
    }

    private void OnDisable()
    {
        shopPlace.ItemChanged -= OnItemChanged;
    }

    private void OnItemChanged(ItemData itemData)
    {
        item = itemData;
        animator.SetBool(isRaisedHash, false);
    }

    public void OnRaised()
    {
        shopPlace.OnRaised();
    }

    public void OnLowered()
    {
        animator.SetBool(isRaisedHash, true);
        itemPlace.SetItem(item);
        shopPlace.UpdateInventory();
    }

    
    public void SetItem(ItemData item)
    {
        itemPlace.SetItem(item);
    }
}
