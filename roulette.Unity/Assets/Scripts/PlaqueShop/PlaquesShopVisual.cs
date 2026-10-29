using UnityEngine;
using Zenject;

public class PlaquesShopVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlaqueShopView plaqueShopView;
    private PlaqueShop plaqueShop;
    private readonly int ejectPlaquesTrigger = Animator.StringToHash("EjectPlaques");


    [Inject] 
    private void Construct(PlaqueShop plaqueShop)
    {
        this.plaqueShop = plaqueShop;
    }

    private void OnEnable()
    {
        plaqueShop.PlaquesBought += OnPlaquesBought;
    }

    private void OnDisable()
    {
        plaqueShop.PlaquesBought -= OnPlaquesBought;

    }
    private void OnPlaquesBought()
    {
        animator.SetTrigger(ejectPlaquesTrigger);
    }

    private void OnAnimationFinished()
    {
        plaqueShopView.ToggleMouseInput(true);
    }
}
