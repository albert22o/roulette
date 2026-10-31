using System.Collections;
using UnityEngine;
using Zenject;

public class PlaqueShopView : MonoBehaviour
{
    [SerializeField] private PlaqueItem[] plaquePlaces;
    [SerializeField] private PlaqueVendingMachine vendingMachine;
    [SerializeField] private MouseInteraction mouseInteraction;
    [SerializeField] private SimpleTooltip tooltip;
    private CancelInputHandler cancelInputHandler;
    [SerializeField] private float cancelDelay = 2f;
    private PlaqueShop plaqueShop;
    private PlaqueUseController plaqueController;
    public bool PlaquesBought { get; private set; }

    [Inject]
    public void Construct(PlaqueShop plaqueShop, PlaqueUseController plaqueController, CancelInputHandler cancelInputHandler)
    {
        this.plaqueShop = plaqueShop;
        this.plaqueController = plaqueController;
        this.cancelInputHandler = cancelInputHandler;
    }

    private void OnEnable()
    {
        foreach (var item in plaquePlaces)
        {
            item.Clicked += OnPlaqueClicked;
            item.HoverEntered += OnPlaqueHoverEnter;
            item.HoverExited += OnPlaqueHoverExit;
        }
        vendingMachine.Clicked += OnPlaqueMachineClick;
        plaqueController.PlaqueUsed += OnPlaqueUsed;
    }

    private void OnDisable()
    {
        foreach (var item in plaquePlaces)
        {
            item.Clicked -= OnPlaqueClicked;
            item.HoverEntered -= OnPlaqueHoverEnter;
            item.HoverExited -= OnPlaqueHoverExit;
        }
        vendingMachine.Clicked -= OnPlaqueMachineClick;
        plaqueController.PlaqueUsed -= OnPlaqueUsed;
    }


    public void TryBuyPlaques()
    {
        if (plaqueShop.TrySpendBossChip() == true)
        {
            var boughtItems = plaqueShop.GetThreeRandomPlaques();
            for (int i = 0; i < boughtItems.Length; i++)
            {
                plaquePlaces[i].SetData(boughtItems[i]);
            }
            ToggleMouseInput(false);
            cancelInputHandler.Toggle(false);
            PlaquesBought = true;
        }
    }

    private void OnPlaqueHoverEnter(PlaqueItem item)
    {
        tooltip.Show(item.Data.Name, item.Data.Description, item.transform.position);
    }

    private void OnPlaqueHoverExit(PlaqueItem item)
    {
        tooltip.Hide();
    }

    private void OnPlaqueClicked(PlaqueItem item)
    {
        cancelInputHandler.Toggle(false);
        plaqueController.UsePlaque(item.Data as PlaqueData);
    }

    public void StartInteraction()
    {
        mouseInteraction.enabled = true;
        cancelInputHandler.Toggle(true);
    }

    public void FinishInteraction()
    {
        mouseInteraction.enabled = false;
        cancelInputHandler.Toggle(false);
    }

    public void ToggleMouseInput(bool value)
    {
        mouseInteraction.enabled = value;
    }

    public void OnPlaqueMachineClick()
    {
        TryBuyPlaques();
    }

    public void OnPlaqueUsed()
    {
        for (int i = 0; i < plaquePlaces.Length; i++)
        {
            plaquePlaces[i].SetData(null);
        }
        StartCoroutine(CancelDelay());
        PlaquesBought = false;
    }

    private IEnumerator CancelDelay()
    {
        yield return new WaitForSeconds(cancelDelay);
        cancelInputHandler.enabled = true;
    }
}

