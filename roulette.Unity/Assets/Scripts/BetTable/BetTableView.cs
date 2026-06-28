using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using static GameStateManager;

class BetTableView : MonoBehaviour
{
    [SerializeField] TextMeshPro availableSumText;
    [SerializeField] TextMeshPro betText;
    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] TextMeshPro generalMultiplierText;
    [SerializeField] float timeBetweenHighlights = 1f;
    [SerializeField] float timeBetweenChipAnimations = 0.5f;
    [SerializeField] float timeBeforeBetsResults = 3f;

    [SerializeField] BottomSectionZone[] bottomSectionColorZones;
    [SerializeField] BottomSectionZone[] bottomSectionDozenZones;

    private BetTable betTable;
    private TableConfig tableConfig;
    private RoundStateManager roundStateManager;
    private BetCalculator betCalculator;
    private ChipHolder chipHolder;
    private int generalMultiplier;

    private GameStateManager gameStateManager;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig, BetCalculator betCalculator,
        RoundStateManager roundStateManager, ChipHolder chipHolder, GameStateManager gameStateManager)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
        this.betCalculator = betCalculator;
        this.roundStateManager = roundStateManager;
        this.chipHolder = chipHolder;
        this.gameStateManager = gameStateManager;
    }

    public void Start()
    {
        if (availableSumText == null)
        {
            Debug.LogError("Не все зависимости проставлены");
            return;
        }
        betTable.ChangeAvailableSum += OnChangeAvailableSum;
        OnChangeAvailableSum(betTable.AvailableSum);

        if (betButtons.Count < tableConfig.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        betTable.ChangeAmountByIndex += OnChangeAmountByIndex;
        tableConfig.PocketNumberChanged += OnPocketNumberChanged;
        tableConfig.PocketColorChanged += OnPocketColorChanged;
        betCalculator.HighlightButtons += OnHighlightButton;
        gameStateManager.GameStateChanged += OnGameStateChanged;


        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => betTable.AddBet(pocket.Key),
                () => betTable.RemoveBet(pocket.Key));
        }

        if (gameStateManager.CurrentState is not GameState.FieldsSelecting) gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        betCalculator.HighlightButtons -= OnHighlightButton;
        betTable.ChangeAmountByIndex -= OnChangeAmountByIndex;
        tableConfig.PocketNumberChanged -= OnPocketNumberChanged;
        tableConfig.PocketColorChanged -= OnPocketColorChanged;
        betTable.ChangeAvailableSum -= OnChangeAvailableSum;
        gameStateManager.GameStateChanged -= OnGameStateChanged;
    }

    private void OnChangeAvailableSum(int newSum)
    {
        availableSumText.text = newSum.ToString();
    }

    private void OnPocketColorChanged(int index, ColorType color)
    {
        betButtons[index].Color = color;
    }

    private void OnPocketNumberChanged(int index, int fieldNum)
    {
        betButtons[index].FieldNum = fieldNum;
    }

    private void OnChangeAmountByIndex(int index, int amount)
    {
        betButtons[index].Amount = amount;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state is GameState.FieldsSelecting)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    private void OnHighlightButton(HitInfo hitInfo)
    {
        StartCoroutine(ResultDisplaySequence(hitInfo));
    }

    private IEnumerator ResultDisplaySequence(HitInfo hitInfo)
    {
        int betSum = 0;
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
            betSum += amount;
        betText.text = betSum.ToString();

        yield return new WaitForSeconds(timeBeforeBetsResults);

        foreach ((var betInfo, var _) in betTable.NotNullBetAmounts)
        {
            if (hitInfo.FieldColor != betInfo.FieldColor && hitInfo.Dozen != betInfo.Dozen)
                continue;

            HighlightButtonsWhere(button => button.Color == hitInfo.FieldColor, BetButton.HighlightPhase.Color);
            HighlightBottomSectionColor(hitInfo);
            DisplayMultiplier(betCalculator.ColorMultiplier);

            yield return new WaitForSeconds(timeBetweenHighlights);

            if (hitInfo.Dozen != betInfo.Dozen)
            {
                yield return ApplyChips(betInfo, hitInfo);
                betCalculator.StartMoneyCounting(betSum, generalMultiplier, hitInfo);
                continue;
            }

            HighlightButtonsWhere(button => button.Dozen == hitInfo.Dozen, BetButton.HighlightPhase.Dozen);
            HighlightBottomSectionDozen(hitInfo);
            DisplayMultiplier(betCalculator.DozenMultiplier);

            yield return new WaitForSeconds(timeBetweenHighlights);

            if (hitInfo.FieldNumber != betInfo.FieldNumber)
            {
                yield return ApplyChips(betInfo, hitInfo);
                betCalculator.StartMoneyCounting(betSum, generalMultiplier, hitInfo);
                continue;
            }

            HighlightButtonsWhere(button => button.FieldNum == hitInfo.FieldNumber, BetButton.HighlightPhase.Number);
            if (hitInfo.FieldNumber == 0)
                DisplayMultiplier(betCalculator.ZeroMultiplier);
            else
                DisplayMultiplier(betCalculator.NumMultiplier);
            
            yield return new WaitForSeconds(timeBetweenHighlights);

            yield return ApplyChips(betInfo, hitInfo);
            betCalculator.StartMoneyCounting(betSum, generalMultiplier, hitInfo);
        }
        ResetTable();
    }

    private void ResetTable()
    {
        ResetButtonColors();

        generalMultiplier = 0;
        generalMultiplierText.text = "";
        betText.text = "";

        betTable.ClearBets();
    }

    private IEnumerator ApplyChips(BetInfo betInfo, HitInfo hitInfo)
    {
        for (int i = 0; i < chipHolder.Capacity; i++)
        {
            chipHolder.ApplyByIndex(i);
            DisplayMaxWinningMultiplier(hitInfo, betInfo);

            yield return new WaitForSeconds(timeBetweenChipAnimations);
        }
    }

    private void HighlightButtonsWhere(Func<BetButton, bool> predicate, BetButton.HighlightPhase phase)
    {
        foreach (BetButton button in betButtons)
        {
            if (predicate(button))
                button.Highlight(true, phase);
        }
    }

    private void HighlightBottomSectionColor(HitInfo hitInfo)
    {
        foreach (BottomSectionZone zone in bottomSectionColorZones)
        {
            if (zone.Color == hitInfo.FieldColor)
                zone.Highlight(true);
        }
    }

    private void HighlightBottomSectionDozen(HitInfo hitInfo)
    {
        foreach (BottomSectionZone zone in bottomSectionDozenZones)
        {
            if (zone.Dozen == hitInfo.Dozen)
                zone.Highlight(true);
        }
    }

    private void ResetButtonColors()
    {
        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionColorZones)
             bottomSectionZone.Highlight(false);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionDozenZones)
            bottomSectionZone.Highlight(false);
    }

    private void DisplayMultiplier(int multiplier)
    {
        generalMultiplier = multiplier;
        generalMultiplierText.text = "x" + generalMultiplier.ToString();
    }

    private void DisplayMaxWinningMultiplier(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.FieldColor == betInfo.FieldColor)
        {
            generalMultiplier = Mathf.Max(betCalculator.GetEffectiveColorMultiplier(hitInfo.FieldColor), generalMultiplier);
            if (hitInfo.Dozen == betInfo.Dozen)
            {
                generalMultiplier = Mathf.Max(betCalculator.GetEffectiveDozenMultiplier(hitInfo.Dozen), generalMultiplier);
                if (hitInfo.FieldNumber == betInfo.FieldNumber)
                {
                    generalMultiplier = betInfo.FieldNumber == 0
                        ? Mathf.Max(betCalculator.GetFieldMultiplier(0, betCalculator.ZeroMultiplier), generalMultiplier)
                        : Mathf.Max(betCalculator.GetFieldMultiplier(hitInfo.FieldNumber, betCalculator.NumMultiplier), generalMultiplier);
                }
            }
        }

        generalMultiplierText.text = "x" + generalMultiplier.ToString();
    }
}