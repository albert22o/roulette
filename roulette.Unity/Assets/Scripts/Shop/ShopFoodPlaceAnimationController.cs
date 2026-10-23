using System;
using UnityEngine;

public class ShopFoodPlaceAnimationController : MonoBehaviour
{
    [SerializeField] private ShopFoodPlace foodPlace;
    [SerializeField] private Animator animator;

    private readonly int isHoverHash = Animator.StringToHash("IsHover");
    private readonly int hasItemHash = Animator.StringToHash("HasItem");

    public event Action CapRised;

    private void OnEnable()
    {
        foodPlace.IsHoverChanged += OnHoverChanged;
        foodPlace.HasItemChanged += OnHasItemChanged;
    }

    private void OnDisable()
    {
        foodPlace.IsHoverChanged -= OnHoverChanged;
        foodPlace.HasItemChanged -= OnHasItemChanged;
    }

    private void OnHoverChanged(bool isHover)
    {
        animator.SetBool(isHoverHash, isHover);
    }

    private void OnHasItemChanged(bool hasItem)
    {
        animator.SetBool(hasItemHash, hasItem);
    }

    public void OnCapRise() => CapRised?.Invoke();
}
