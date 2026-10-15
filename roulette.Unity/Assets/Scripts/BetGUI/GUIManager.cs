using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private UICounter goldenChipsCounter, silverChipsCounter, bossChipsCounter;
    [SerializeField] private BetsGUIDisplay betsDisplay;
    [SerializeField] private ProgressBarGUI quoteProgress;
    [SerializeField] private TextMeshProUGUI rerollPenalty;
    [SerializeField] private Slider rerollPenaltySlider;
    [SerializeField] private GameObject betPanel;

    private GoldenChips goldenChips;
    private BossChips bossChips;
    private RoundManager roundManager;
    private TableInfo tableInfo;
    private BetCalculator betCalculator;
    private GameStateManager gameStateManager;
    private BetTable betTable;


    [Inject]
    private void Construct(GoldenChips goldenChips, RoundManager roundManager, TableInfo tableInfo, BossChips bossChips,
        BetCalculator betCalculator, GameStateManager gameStateManager, BetTable betTable)
    {
        this.goldenChips = goldenChips;
        this.roundManager = roundManager;
        this.tableInfo = tableInfo;
        this.bossChips = bossChips;
        this.betCalculator = betCalculator;
        this.gameStateManager = gameStateManager;
        this.betTable = betTable;

        goldenChips.AmountChanged += OnGoldenChipsChanged;
        roundManager.BetCountChanged += OnBetCountChanged;
        bossChips.AmountChanged += OnBossChipsChanged;
        betTable.ChangeAvailableSum += OnSilverChipsChanged;

        roundManager.QuoteChanged += OnQuoteOrGoalChanged;
        roundManager.GoalChanged += OnQuoteOrGoalChanged;
        betCalculator.RerollCoeffChanged += OnRerollCoeffChanged;

        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {

        OnBetCountChanged(roundManager.BetCount);
        OnQuoteOrGoalChanged(roundManager.Quote);
        OnGoldenChipsChanged(goldenChips.Amount);
        OnBossChipsChanged(bossChips.Amount);
        OnRerollCoeffChanged(betCalculator.RerollCoeff);
        OnSilverChipsChanged(betTable.AvailableSum);
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        betPanel.SetActive(state is GameStateManager.GameState.BettingСycle);
    }

    private void OnRerollCoeffChanged(float rerollCoef)
    {
        rerollPenaltySlider.value = 1 - rerollCoef;
        rerollPenalty.text = $"{(1 - rerollCoef) * 100}/100";
    }

    private void OnDestroy()
    {
        goldenChips.AmountChanged -= OnGoldenChipsChanged;
        roundManager.BetCountChanged -= OnBetCountChanged;
        bossChips.AmountChanged -= OnBossChipsChanged;
        betTable.ChangeAvailableSum -= OnSilverChipsChanged;

        roundManager.QuoteChanged -= OnQuoteOrGoalChanged;
        roundManager.GoalChanged -= OnQuoteOrGoalChanged;
        betCalculator.RerollCoeffChanged -= OnRerollCoeffChanged;

        gameStateManager.GameStateChanged -= OnGameStateChanged;
    }

    private void OnBetCountChanged(int remainingBets)
    {
        betsDisplay.SetRemainingBets(remainingBets, tableInfo.BetCount);
    }

    private void OnGoldenChipsChanged(int amount)
    {
        goldenChipsCounter.SetCount(amount);
    }

    private void OnBossChipsChanged(int amount)
    {
        bossChipsCounter.SetCount(amount);
    }

    private void OnQuoteOrGoalChanged(int _)
    {
        quoteProgress.SetCurrentAndMaxValue(roundManager.Quote, roundManager.Goal);
    }

    private void OnSilverChipsChanged(int money)
    {
        silverChipsCounter.SetCount(money);
    }
}
