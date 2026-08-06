using System;
using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{

    [SerializeField] private ShopTooltip tooltip;

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
    private RoundManager roundManager;
    private ProgressionManager progressionManager;

    public event Action InteractionStarted, InteractionFinished;

    [Inject]
    public void Construct(GoldenChips goldenChips, BallHolder ballHolder, ChipHolder chipHolder,
        RoundManager roundManager, ProgressionManager progressionManager)
    {
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
        this.roundManager = roundManager;
        this.progressionManager = progressionManager;
        roundManager.Win += OnQuotaUpdated;
    }

    private void Start()
    {
        UpdateShop(true);
    }


    public void OnQuotaUpdated()
    {
        UpdateShop(false);
    }

    private void UpdateShop(bool forceUpdateFood)
    {
        if (progressionManager.Current.IsBoss|| forceUpdateFood)
        {
            foodPlace.UpdateItem();
        }
        chipPlacesController.Fill();
        rerollButton.ResetCost();
        baseMoneyVendingMachine.ResetCost();
    }

    public void ShowTooltip(string title, string mainText, Vector3 position)
    {
        tooltip.Show(title, mainText,position);
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.Hide();
    }

    public bool TrySell(ItemData item, int index)
    {
        if (item is BallInfoData ballData)
        {
            int undestroyableBalls = 0;
            foreach (var ball in ballHolder.Balls)
            {
                if (!ball.BallInfoData.IsDestroyable) undestroyableBalls++;
            }
            if (undestroyableBalls > 1 || ballData.IsDestroyable)
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

    public int GetSellPrice(ItemData item)
    {
        if (item is BallInfoData)
        {
            return ballSellCost;
        }
        else if (item is ChipData chipData)
        {
            int sellCost = (int)(chipData.Cost * chipSellMultiplier);
            return sellCost;
        }
        else
        {
            return 0;
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
