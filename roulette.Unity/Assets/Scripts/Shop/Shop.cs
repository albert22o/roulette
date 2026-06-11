using System;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class Shop : MonoBehaviour
{

    [SerializeField] private ShopTooltip tooltip;

    [SerializeField] private float tooltipHeight;
    [SerializeField] private float chipSellMultiplier = 0.5f;
    [SerializeField] private int ballSellCost = 3;

    [SerializeField] private ShopRerollButton rerollButton;
    [SerializeField] private BaseMoneyVendingMachine baseMoneyVendingMachine;
    [SerializeField] private BallsVendingMachine ballsVendingMachine;
    [SerializeField] private ShopFoodPlace foodPlace;
    [SerializeField] private MouseInteraction shopInteraction;
    [SerializeField] private ShopChipPlacesController chipPlacesController;
    [SerializeField] private ShopInputHandler inputHandler;


    private GoldenChips goldenChips;
    private Inventory inventory;


    public event Action InteractionStarted, InteractionFinished;


    [Inject]
    public void Construct(GoldenChips goldenChips, Inventory inventory)
    {
        this.goldenChips = goldenChips;
        this.inventory = inventory;
    }

   

    private void Start()
    {
        OnQuotaUpdated();
    }

    public void OnQuotaUpdated()
    {
        chipPlacesController.Fill();
        rerollButton.ResetCost();
        baseMoneyVendingMachine.ResetCost();
        foodPlace.UpdateItem();
    }

    public void ShowTooltip(string title, string mainText, Vector3 position)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetData(title, mainText);
        tooltip.transform.position = position + Vector3.up * tooltipHeight;
    }

    public void HIdeTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }

    public void Sell(Item item)
    {
        if (item.Data is BallShopItemData)
        {
            if (inventory.Balls.Count > 1)
            {
                goldenChips.Add(ballSellCost);
                inventory.RemoveItem(item);
            }
        }
        else if (item.Data is ChipShopItemData chipData)
        {
            goldenChips.Add((int)(chipData.Cost * chipSellMultiplier));
            inventory.RemoveItem(item);
        }
    }

    public void SetInteractionAvailable(bool value)
    {
        shopInteraction.enabled = value;
        inputHandler.enabled = value;
    }
    public void StartInteraction()
    {
        SetInteractionAvailable(true);
        InteractionStarted?.Invoke();
    }

    public void FinishInteraction()
    {
        SetInteractionAvailable(false);
        InteractionFinished?.Invoke();
    }
}
