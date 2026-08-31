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
    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] float timeBetweenHighlights = 1f;
    [SerializeField] float timeBeforeBetsHighlight = 3f;
    [SerializeField] private ShopTooltip tooltip;
    [Header("Сгорание фишек")]
    [SerializeField] private float burnStartDelay = 1f;
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float burnTime = 2f;

    [SerializeField] BottomSectionZone[] bottomSectionColorZones;
    [SerializeField] BottomSectionZone[] bottomSectionDozenZones;

    private BetTable betTable;
    private TableInfo tableInfo;
    private BetCalculator betCalculator;
    private WinningSequence winningSequence;
    private ColorsDescription colorsDescription;
    private BallResults ballResults;

    [Inject]
    public void Construct(BetTable betTable, TableInfo tableInfo, BetCalculator betCalculator, WinningSequence winningSequence,
        ColorsDescription colorsDescription, BallResults ballResults)
    {
        this.betTable = betTable;
        this.tableInfo = tableInfo;
        this.betCalculator = betCalculator;
        this.winningSequence = winningSequence;
        this.colorsDescription = colorsDescription;
        this.ballResults = ballResults;
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
        ballResults.GrayBallHit += OnGrayBallHit; 

        if (betButtons.Count < tableInfo.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        betTable.ChangeAmountByIndex += OnChangeAmountByIndex;
        tableInfo.PocketNumberChanged += OnPocketNumberChanged;
        tableInfo.PocketColorChanged += OnPocketColorChanged;

        winningSequence.Highlight += OnHighlight;
        winningSequence.EndSequence += OnEndSequence;

        foreach (var pocket in tableInfo.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => betTable.AddBet(pocket.Key),
                () => betTable.RemoveBet(pocket.Key),
                pocket.Key);
        }
        foreach( var button in betButtons)
        {
            button.HoverEnter += OnHoverEnter;
            button.HoverExit += OnHoverExit;
        }
    }

    private void OnHoverExit(BetButton button)
    {
        if (colorsDescription.GetColorData(button.Color) != null)
        {
            tooltip.Hide();
        }
    }

    private void OnHoverEnter(BetButton button)
    {
        var colorData = colorsDescription.GetColorData(button.Color);
        if (colorData != null)
        {
            tooltip.Show(colorData.ColorName, colorData.ColorDescription, button.transform.position);
        }
    }

    private void OnEnable()
    {
        UpdateAllButtons();
    }

    private void OnDestroy()
    {
        betTable.ChangeAmountByIndex -= OnChangeAmountByIndex;
        tableInfo.PocketNumberChanged -= OnPocketNumberChanged;
        tableInfo.PocketColorChanged -= OnPocketColorChanged;
        betTable.ChangeAvailableSum -= OnChangeAvailableSum;
        ballResults.GrayBallHit -= OnGrayBallHit; 

        foreach (var button in betButtons)
        {
            if (button == null) continue;
            button.HoverEnter += OnHoverEnter;
            button.HoverExit += OnHoverExit;
        }
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

    private void UpdateAllButtons()
    {
        for (int i = 0; i < betButtons.Count; i++)
        {
            betButtons[i].Color = tableInfo.PocketMapping[i].FieldColor;
            betButtons[i].FieldNum = tableInfo.PocketMapping[i].FieldNumber;
        }
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
            winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyBall);
            yield break;
        }
    
        HighlightButtonsWhere(button => button.Color == hitInfo.FieldColor, BetButton.HighlightPhase.Color);
        HighlightBottomSectionColor(hitInfo);
        winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.ColorMultiplier);

        if (hitInfo.Dozen != betInfo.Dozen)
        {
            winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyBall);
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenHighlights);

        HighlightButtonsWhere(button => button.Dozen == hitInfo.Dozen, BetButton.HighlightPhase.Dozen);
        HighlightBottomSectionDozen(hitInfo);
        winningSequence.CompareAndDisplayMaxMultiplier(betCalculator.DozenMultiplier);

        if (hitInfo.FieldNumber != betInfo.FieldNumber)
        {
            winningSequence.CallPhaseExecution(WinningSequence.SequencePhase.ApplyBall);
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

    private void OnEndSequence(bool isRoundWon)
    {
        if (!isRoundWon)
            return;

        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionColorZones)
             bottomSectionZone.Highlight(false);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionDozenZones)
            bottomSectionZone.Highlight(false);
    }
    private void OnGrayBallHit(HitInfo hitInfo)
    {
        foreach (var button in betButtons)
        {
            if (button != null)
                button.BurnChips(burnMaterial, burnStartDelay, burnTime);
        }
    }
}