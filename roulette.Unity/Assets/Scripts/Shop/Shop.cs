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
    [SerializeField] private CancelInputHandler inputHandler;

    public int BallSellCost => ballSellCost;

    private GoldenChips goldenChips;
    private BallHolder ballHolder;
    private ChipHolder chipHolder;

    public event Action InteractionStarted, InteractionFinished;

    [Inject]
    public void Construct(GoldenChips goldenChips, BallHolder ballHolder, ChipHolder chipHolder)
    {
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
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
        tooltip.Show(title, mainText);
        tooltip.gameObject.SetActive(true);
        tooltip.transform.position = position + Vector3.up * tooltipHeight;
    }

    public void HideTooltip()
    {
        tooltip.Hide();
    }

    public bool TrySell(ItemData item, int index)
    {
        if (item is BallInfoData)
        {
            if (ballHolder.Balls.Count > 1)
            {
                goldenChips.Add(ballSellCost);
                ballHolder.RemoveBall(index);
                return true;
            }
            return false;
        }
        else if (item is ChipData chipData)
        {
            float sellCost = (int)(chipData.Cost * chipSellMultiplier);
            if (sellCost > 0) goldenChips.Add((int)(chipData.Cost * chipSellMultiplier));
            chipHolder.RemoveChip(index);
            return true;
        }
        return false;
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
