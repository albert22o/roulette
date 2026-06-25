using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using Zenject;

class BetTableView : MonoBehaviour
{
    [SerializeField] TextMeshPro availableSumText;
    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] float timeBetweenHighlights = 1f;
    [SerializeField] float timeBeforeBetsHighlight = 3f;

    [SerializeField] BottomSectionZone[] bottomSectionColorZones;
    [SerializeField] BottomSectionZone[] bottomSectionDozenZones;

    private BetTable betTable;
    private TableConfig tableConfig;
    private BetCalculator betCalculator;
    private WinningSequence winningSequence;
    private int generalMultiplier;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig, BetCalculator betCalculator, WinningSequence winningSequence)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
        this.betCalculator = betCalculator;
        this.winningSequence = winningSequence;
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
        winningSequence.Highlight += OnHighlight;
        winningSequence.EndSequence += OnEndSequence;

        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => betTable.AddBet(pocket.Key),
                () => betTable.RemoveBet(pocket.Key));
        }
    }

    private void OnDestroy()
    {
        betTable.ChangeAmountByIndex -= OnChangeAmountByIndex;
        tableConfig.PocketNumberChanged -= OnPocketNumberChanged;
        tableConfig.PocketColorChanged -= OnPocketColorChanged;
        betTable.ChangeAvailableSum -= OnChangeAvailableSum;
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

    private void OnHighlight(BetInfo betInfo, HitInfo hitInfo)
    {
        StartCoroutine(HighlightSequence(betInfo, hitInfo));
    }

    private IEnumerator HighlightSequence(BetInfo betInfo, HitInfo hitInfo)
    {
        yield return new WaitForSeconds(timeBeforeBetsHighlight);

        if (hitInfo.FieldColor != betInfo.FieldColor && hitInfo.Dozen != betInfo.Dozen)
        { 
            winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.Finish);
            yield break;
        }
    
        HighlightButtonsWhere(button => button.Color == hitInfo.FieldColor, BetButton.HighlightPhase.Color);
        HighlightBottomSectionColor(hitInfo);
        winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.ColorMultiplier);

        if (hitInfo.Dozen != betInfo.Dozen)
        {
            winningSequence.NextPhase();
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenHighlights);

        HighlightButtonsWhere(button => button.Dozen == hitInfo.Dozen, BetButton.HighlightPhase.Dozen);
        HighlightBottomSectionDozen(hitInfo);
        winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.DozenMultiplier);

        if (hitInfo.FieldNumber != betInfo.FieldNumber)
        {
            winningSequence.NextPhase();
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenHighlights);

        HighlightButtonsWhere(button => button.FieldNum == hitInfo.FieldNumber, BetButton.HighlightPhase.Number);
        if (hitInfo.FieldNumber == 0)
            winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.ZeroMultiplier);
        else
            winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.NumMultiplier);

        winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyBall);
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

    private void OnEndSequence()
    {
        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionColorZones)
             bottomSectionZone.Highlight(false);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionDozenZones)
            bottomSectionZone.Highlight(false);
    }
}