using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

class BetTableView : MonoBehaviour
{
    [SerializeField] TextMeshPro availableSumText;
    [SerializeField] TextMeshPro betText;
    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] TextMeshPro generalMultiplierText;
    [SerializeField] float timeBetweenHighlights = 1f;
    [SerializeField] float timeBetweenChipAnimations = 0.5f;
    [SerializeField] float timeForCountingMoney = 3f;
    [SerializeField] float timeBeforeBetsResults = 3f;

    [SerializeField] BottomSectionZone[] bottomSectionColorZones;
    [SerializeField] BottomSectionZone[] bottomSectionDozenZones;

    private BetTable betTable;
    private TableConfig tableConfig;
    private RoundStateManager roundStateManager;
    private BetCalculator betCalculator;
    private ChipHolder chipHolder;
    private int generalMultiplier;
    private int betSum;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig, BetCalculator betCalculator,
        RoundStateManager roundStateManager, ChipHolder chipHolder)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
        this.betCalculator = betCalculator;
        this.roundStateManager = roundStateManager;
        this.chipHolder = chipHolder;
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

        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => betTable.AddBet(pocket.Key),
                () => betTable.RemoveBet(pocket.Key));
        }
    }

    private void OnDestroy()
    {
        betCalculator.HighlightButtons -= OnHighlightButton;
        betTable.ChangeAmountByIndex -= OnChangeAmountByIndex;
        tableConfig.PocketNumberChanged -= OnPocketNumberChanged;
        tableConfig.PocketColorChanged -= OnPocketColorChanged;
        betTable.ChangeAvailableSum -= OnChangeAvailableSum;
    }

    private void OnChangeAvailableSum(int newSum)
    {
        availableSumText.text = newSum.ToString();
    }

    private void OnPocketColorChanged(int index, string color)
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

    private void OnHighlightButton(HitInfo hitInfo)
    {
        StartCoroutine(ResultDisplaySequence(hitInfo));
    }

    private IEnumerator ResultDisplaySequence(HitInfo hitInfo)
    {
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
            betSum += amount;
        betText.text = betSum.ToString();

        yield return new WaitForSeconds(timeBeforeBetsResults);

        foreach ((var betInfo, var _) in betTable.NotNullBetAmounts)
        {
            if (hitInfo.FieldColor != betInfo.FieldColor && hitInfo.Dozen != betInfo.Dozen)
                continue;
                
            ProcessButtonHighlightByColor(hitInfo);
            DisplayMultiplier(betCalculator.ColorMultiplier);

            yield return new WaitForSeconds(timeBetweenHighlights);

            if (hitInfo.Dozen != betInfo.Dozen)
            {
                yield return ApplyChips(betInfo, hitInfo);
                betCalculator.StartMoneyCounting(betSum, generalMultiplier, hitInfo);
                continue;
            }

            ProcessButtonHighlightByDozen(hitInfo);
            DisplayMultiplier(betCalculator.DozenMultiplier);

            yield return new WaitForSeconds(timeBetweenHighlights);

            if (hitInfo.FieldNumber != betInfo.FieldNumber)
            {
                yield return ApplyChips(betInfo, hitInfo);
                betCalculator.StartMoneyCounting(betSum, generalMultiplier, hitInfo);
                continue;
            }

            ProcessButtonHighlightByNumber(hitInfo);
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
        betSum = 0;
        betText.text = "";

        betTable.ClearBets();
    }

    private IEnumerator ApplyChips(BetInfo betInfo, HitInfo hitInfo)
    {
        for (int i = 0; i < chipHolder.Сhips.Length; i++)
        {
            chipHolder.ApplyByIndex(i);
            DisplayMaxWinningMultiplier(hitInfo, betInfo);

            yield return new WaitForSeconds(timeBetweenChipAnimations);
        }
    }

    private void ProcessButtonHighlightByColor(HitInfo hitInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.Color == hitInfo.FieldColor)
                button.Highlight(true, BetButton.HighlightPhase.Color);
        }

        foreach (BottomSectionZone bottomSectionZone in bottomSectionColorZones)
        {
            if (bottomSectionZone.Color == hitInfo.FieldColor)
                bottomSectionZone.Highlight(true);
        }
    }

    private void ProcessButtonHighlightByDozen(HitInfo hitInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.Dozen == hitInfo.Dozen)
                button.Highlight(true, BetButton.HighlightPhase.Dozen);
        }

        foreach (BottomSectionZone bottomSectionZone in bottomSectionDozenZones)
        {
            if (bottomSectionZone.Dozen == hitInfo.Dozen)
                bottomSectionZone.Highlight(true);
        }
    }

    private void ProcessButtonHighlightByNumber(HitInfo hitInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.FieldNum == hitInfo.FieldNumber)
                button.Highlight(true, BetButton.HighlightPhase.Number);
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