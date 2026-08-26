using System;
using TMPro;
using UnityEngine;
using Zenject;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private UICounter goldenChipsCounter, silverChipsCounter;
    [SerializeField] private BetsGUIDisplay betsDisplay;
    [SerializeField] private ProgressBarGUI quoteProgress;
    [SerializeField] private TextMeshProUGUI rerollPenalty;
    [SerializeField] private GameObject betPanel;

    private GoldenChips goldenChips;
    private BossChips bossChips;
    private RoundManager roundManager;
    private TableInfo tableInfo;
    private BetCalculator betCalculator;
    private GameStateManager gameStateManager;


    [Inject]
    private void Construct(GoldenChips goldenChips, RoundManager roundManager, TableInfo tableInfo, BossChips bossChips,
        BetCalculator betCalculator, GameStateManager gameStateManager)
    {
        this.goldenChips = goldenChips;
        this.roundManager = roundManager;
        this.tableInfo = tableInfo;
        this.bossChips = bossChips;
        this.betCalculator = betCalculator;
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        goldenChips.AmountChanged += OnGoldenChipsChanged;
        roundManager.BetCountChanged += OnBetCountChanged;
        bossChips.AmountChanged += OnSilverChipsChanged;

        roundManager.QuoteChanged += OnQuoteOrGoalChanged;
        roundManager.GoalChanged += OnQuoteOrGoalChanged;
        betCalculator.RerollCoeffChanged += OnRerollCoeffChanged;

        gameStateManager.GameStateChanged += OnGameStateChanged;

        OnBetCountChanged(roundManager.BetCount);
        OnQuoteOrGoalChanged(roundManager.Quote);
        OnGoldenChipsChanged(goldenChips.Amount);
        OnSilverChipsChanged(bossChips.Amount);
        OnRerollCoeffChanged(betCalculator.RerollCoeff);
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        betPanel.SetActive(state is GameStateManager.GameState.BettingСycle);
    }

    private void OnRerollCoeffChanged(float rerollCoef)
    {
        rerollPenalty.text = $"{rerollCoef * 100}%";
    }

    private void OnDestroy()
    {
        goldenChips.AmountChanged -= OnGoldenChipsChanged;
        roundManager.BetCountChanged -= OnBetCountChanged;
        bossChips.AmountChanged -= OnSilverChipsChanged;

        roundManager.QuoteChanged -= OnQuoteOrGoalChanged;
        roundManager.GoalChanged -= OnQuoteOrGoalChanged;
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

    private void OnSilverChipsChanged(int amount)
    {
        silverChipsCounter.SetCount(amount);
    }

    private void OnQuoteOrGoalChanged(int _)
    {
        float valueInPercent = (float)roundManager.Quote / (float)roundManager.Goal * 100;
        valueInPercent = Mathf.Clamp(valueInPercent, 0, 100);
        quoteProgress.SetValueInPercent(Mathf.FloorToInt(valueInPercent));
    }
}
