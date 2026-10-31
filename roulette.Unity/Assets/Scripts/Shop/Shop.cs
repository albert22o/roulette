using System;
using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{

    [SerializeField] private ShopTooltip tooltip;

    [SerializeField] private ShopRerollButton rerollButton;
    [SerializeField] private BaseMoneyVendingMachine baseMoneyVendingMachine;
    [SerializeField] private BallsVendingMachine ballsVendingMachine;
    [SerializeField] private ShopFoodPlace foodPlace;
    [SerializeField] private MouseInteraction shopInteraction;
    [SerializeField] private ShopChipPlacesController chipPlacesController;
    private CancelInputHandler cancelInputHandler;
    [SerializeField] private ShopConfig config;

    public ShopConfig Config => config;

    private GoldenChips goldenChips;
    private BallHolder ballHolder;
    private ChipHolder chipHolder;
    private ProgressionManager progressionManager;


    public event Action InteractionStarted, InteractionFinished;

    public event Action ItemPurchased, ItemSold;

    [Inject]
    public void Construct(GoldenChips goldenChips, BallHolder ballHolder, ChipHolder chipHolder,
        RoundManager roundManager, ProgressionManager progressionManager, CancelInputHandler cancelInputHandler)
    {
        this.goldenChips = goldenChips;
        this.ballHolder = ballHolder;
        this.chipHolder = chipHolder;
        this.progressionManager = progressionManager;
        this.cancelInputHandler = cancelInputHandler;
        roundManager.Win += OnWin;
    }

    private void Start()
    {
        UpdateShop(true);
        baseMoneyVendingMachine.ResetCost();
        baseMoneyVendingMachine.BaseMoneyPurchased += OnItemPurchased;
        ballsVendingMachine.BallBought += OnBallBought;
        foodPlace.FoodPurchased += OnFoodBought;
        chipPlacesController.ChipBought += OnChipBought;
        rerollButton.RerollUsed += OnItemPurchased;
    }

    private void OnChipBought()
    {
        ItemPurchased?.Invoke();
    }

    private void OnFoodBought()
    {
        OnItemPurchased();
    }

    private void OnBallBought(BallInfoData data)
    {
        OnItemPurchased();
    }

    public void OnWin()
    {
        UpdateShop(false);
    }

    private void UpdateShop(bool forceUpdate)
    {
        if (progressionManager.Current.IsBoss || forceUpdate)
        {
            foodPlace.UpdateItem();
            ballsVendingMachine.ResetSpinAmount();
        }
        chipPlacesController.Fill();
        rerollButton.ResetCost();
    }

    public void ShowTooltip(string title, string mainText, string cost, Vector3 position, RarityType rarityType = RarityType.None)
    {
        tooltip.Show(title, mainText, cost, rarityType, position);
        tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.Hide();
    }

    public bool TrySell(ItemData item)
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
                goldenChips.Add(Config.BallSellCost);
                ballHolder.RemoveBall(ballData);
                ItemSold?.Invoke();
                return true;
            }
            return false;
        }
        else if (item is ChipData chipData)
        {
            float sellCost = (int)(chipData.Cost * config.ChipSellMultiplier);
            if (sellCost > 0) goldenChips.Add((int)(chipData.Cost * config.ChipSellMultiplier));
            chipHolder.RemoveChip(chipData);
            ItemSold?.Invoke();
            return true;
        }
        return false;
    }

    public int GetSellPrice(ItemData item)
    {
        if (item is BallInfoData)
        {
            return Config.BallSellCost;
        }
        else if (item is ChipData chipData)
        {
            int sellCost = (int)(chipData.Cost * Config.ChipSellMultiplier);
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
        cancelInputHandler.Toggle(value);
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

    public void OnItemPurchased() => ItemPurchased?.Invoke();
}
