using UnityEngine;
using Zenject;

public class PlaqueShopView : MonoBehaviour
{
    [SerializeField] private PlaqueItem[] plaquePlaces;
    [SerializeField] private PlaqueVendingMachine vendingMachine;
    [SerializeField] private MouseInteraction mouseInteraction;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private float tooltipHeight;
    [SerializeField] private CancelInputHandler cancelInputHandler;
    private PlaqueShop plaqueShop;
    private PlaqueUseController plaqueController;

    [Inject]
    public void Construct(PlaqueShop plaqueShop, PlaqueUseController plaqueController)
    {
        this.plaqueShop = plaqueShop;
        this.plaqueController = plaqueController;
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
        }
    }

    private void OnPlaqueHoverEnter(PlaqueItem item)
    {
        tooltip.SetData(item.Data.Name, item.Data.Description);
        tooltip.transform.position = item.transform.position + Vector3.up * tooltipHeight;
        tooltip.gameObject.SetActive(true);
    }

    private void OnPlaqueHoverExit(PlaqueItem item)
    {
        tooltip.gameObject.SetActive(false);
    }

    private void OnPlaqueClicked(PlaqueItem item)
    {
        cancelInputHandler.enabled = false;
        plaqueController.UsePlaque(item.Data);
    }

    public void StartInteraction()
    {
        mouseInteraction.enabled = true;
        cancelInputHandler.enabled = true;
    }

    public void FinishInteraction()
    {
        mouseInteraction.enabled = false;
        cancelInputHandler.enabled = false;
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
        cancelInputHandler.enabled = true;
    }
}

