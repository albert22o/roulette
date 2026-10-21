using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BetTableView : MonoBehaviour
{
    public IReadOnlyList<BetButton> Buttons => betButtons;
    public IReadOnlyList<BottomSectionZone> ColorZones => bottomSectionColorZones;
    public IReadOnlyList<BottomSectionZone> DozenZones => bottomSectionDozenZones;

    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] float timeBetweenHighlights = 1f;
    [SerializeField] float timeBeforeBetsHighlight = 3f;
    [SerializeField] private ShopTooltip tooltip;
    [SerializeField] private float betsClearDelay = 2f;

    [SerializeField] BottomSectionZone[] bottomSectionColorZones;
    [SerializeField] BottomSectionZone[] bottomSectionDozenZones;

    [Header("Default Burn Settings")]
    [SerializeField] private Material defaultBurnMaterial;
    [SerializeField] private float defaultBurnDuration = 1.5f;
    [SerializeField] private string defaultDissolveProperty = "_Disolve";

    private BetTable betTable;
    private TableInfo tableInfo;
    private BetCalculator betCalculator;
    private WinningSequence winningSequence;
    private ColorsDescription colorsDescription;
    private BallResults ballResults;
    private bool isBetAvailable = true;

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
        if (betButtons.Count < tableInfo.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        betTable.ChangeAmountByIndex += OnChangeAmountByIndex;
        tableInfo.PocketNumberChanged += OnPocketNumberChanged;
        tableInfo.PocketColorChanged += OnPocketColorChanged;
        betTable.BetsCleared += OnBetsCleared;

        winningSequence.Highlight += OnHighlight;
        winningSequence.EndSequence += OnEndSequence;

        foreach (var pocket in tableInfo.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value, pocket.Key);
        }
        foreach (var button in betButtons)
        {
            button.HoverEnter += OnHoverEnter;
            button.HoverExit += OnHoverExit;
            button.RightClick += OnRightButtonClick;
            button.LeftClick += OnLeftButtonClick;
        }
    }

    private void OnRightButtonClick(BetButton button)
    {
        if (isBetAvailable)
            betTable.RemoveBet(button.Index);
    }

    private void OnLeftButtonClick(BetButton button)
    {
        if (isBetAvailable)
            betTable.AddBet(button.Index);
    }

    private void OnHoverExit(BetButton button)
    {
        if (colorsDescription.GetColorData(button.Color) != null)
            tooltip.Hide();
    }

    private void OnHoverEnter(BetButton button)
    {
        var colorData = colorsDescription.GetColorData(button.Color);
        if (colorData != null)
            tooltip.Show(colorData.ColorName, colorData.ColorDescription, button.transform.position);
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
        betTable.BetsCleared -= OnBetsCleared;

        foreach (var button in betButtons)
        {
            if (button == null) continue;
            button.HoverEnter -= OnHoverEnter;
            button.HoverExit -= OnHoverExit;
        }
    }

    private void OnBetsCleared(bool isImmediately)
    {
        if (isImmediately)
            DeleteAllBetChipImmediately();
        else
            StartCoroutine(DeleteAllBetChips());
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

    private IEnumerator DeleteAllBetChips()
    {
        isBetAvailable = false;

        yield return new WaitForSeconds(betsClearDelay);

        float maxBurnDuration = 0f;
        foreach (var button in betButtons)
        {
            float d = button.BurnAllChips(defaultBurnMaterial, defaultBurnDuration, defaultDissolveProperty);
            if (d > maxBurnDuration)
                maxBurnDuration = d;
        }

        if (maxBurnDuration > 0f)
            yield return new WaitForSeconds(maxBurnDuration);

        DeleteAllBetChipImmediately();
        isBetAvailable = true;
    }

    private void DeleteAllBetChipImmediately()
    {
        foreach (var button in betButtons)
            button.ClearChipsImmediately();
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

    private void OnEndSequence(EndSequenceType endSequenceType)
    {
        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionColorZones)
            bottomSectionZone.Highlight(false);

        foreach (BottomSectionZone bottomSectionZone in bottomSectionDozenZones)
            bottomSectionZone.Highlight(false);
    }
}
