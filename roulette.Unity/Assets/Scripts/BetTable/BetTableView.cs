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
    [SerializeField] TextMeshPro betText;
    [SerializeField] List<BetButton> betButtons = new();
    [SerializeField] TextMeshPro generalMultiplierText;
    [SerializeField] float timeBetweenHighlights = 1f;

    [SerializeField] SpriteRenderer firstDozenSprite;
    [SerializeField] SpriteRenderer secondDozenSprite;
    [SerializeField] SpriteRenderer thirdDozenSprite;
    [SerializeField] SpriteRenderer redNumberSprite;
    [SerializeField] SpriteRenderer blackNumberSprite;

    [SerializeField] Color colorHighlight;
    [SerializeField] Color dozenHighlight;
    [SerializeField] Color numberHighlight;
    [SerializeField] Color lowlight;

    private BetTable betTable;
    private TableConfig tableConfig;
    private RoundStateManager roundStateManager;
    private BetCalculator betCalculator;
    private float generalMultiplier;
    private int betSum;
    private Color baseBottomSectionColor;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig, BetCalculator betCalculator, RoundStateManager roundStateManager)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
        this.betCalculator = betCalculator;
        betCalculator.HighlightButtons += OnHighlightButton;
        this.roundStateManager = roundStateManager;
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

        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => betTable.AddBet(pocket.Key),
                () => betTable.RemoveBet(pocket.Key));
        }
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

    private void OnHighlightButton(HitInfo hitInfo, int reward)
    {
        StartCoroutine(ResultDisplaySequence(hitInfo, reward));
    }

    public IEnumerator ResultDisplaySequence(HitInfo hitInfo, int reward)
    {
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);
        baseBottomSectionColor = firstDozenSprite.color;
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
            betSum += amount;
        betText.text = betSum.ToString();

        yield return new WaitForSeconds(3f);

        //Для каждой ненулевой ставки
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            if (hitInfo.FieldColor == betInfo.FieldColor || hitInfo.Dozen == betInfo.Dozen)
            {
                ProcessButtonHighlightByColor(hitInfo, betInfo);
                ProcessBottomSectionHighlightByColor(hitInfo, betInfo);
                DisplayColorMultiplier(hitInfo, betInfo);

                if (hitInfo.Dozen == betInfo.Dozen)
                {
                    yield return new WaitForSeconds(timeBetweenHighlights);

                    ProcessButtonHighlightByDozen(hitInfo, betInfo);
                    ProcessBottomSectionHighlightByDozen(hitInfo, betInfo);
                    DisplayDozenMultiplier(hitInfo, betInfo);

                    if (hitInfo.FieldNumber == betInfo.FieldNumber)
                    {
                        yield return new WaitForSeconds(timeBetweenHighlights);

                        ProcessButtonHighlightByNumber(hitInfo, betInfo);
                        DisplayNumberMultiplier(hitInfo, betInfo);
                    }
                }
            }
        }

        yield return new WaitForSeconds(3f);

        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);
        ResetBottomSectionColors();

        generalMultiplier = 0;
        generalMultiplierText.text = "";
        betSum = 0;
        betText.text = "";

        betCalculator.GetRewardAction();
        betTable.ClearBets();
    }

    private void ProcessButtonHighlightByColor(HitInfo hitInfo, BetInfo betInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.Color == betInfo.FieldColor)
                button.Highlight(true, BetButton.HighlightPhase.Color);
        }
    }

    private void ProcessButtonHighlightByDozen(HitInfo hitInfo, BetInfo betInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.Dozen == betInfo.Dozen)
                button.Highlight(true, BetButton.HighlightPhase.Dozen);
        }
    }

    private void ProcessButtonHighlightByNumber(HitInfo hitInfo, BetInfo betInfo)
    {
        foreach (BetButton button in betButtons)
        {
            if (button.FieldNum == betInfo.FieldNumber)
                button.Highlight(true, BetButton.HighlightPhase.Number);
        }
    }

    private void ProcessBottomSectionHighlightByDozen(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.Dozen != DozenType.Zero)
        {
            SpriteRenderer spriteToColor = hitInfo.Dozen switch
            {
                DozenType.First => firstDozenSprite,
                DozenType.Second => secondDozenSprite,
                DozenType.Third => thirdDozenSprite,
                _ => null
            };

            if (hitInfo.Dozen == betInfo.Dozen)
                spriteToColor.color = dozenHighlight;
        }
    }

    private void ProcessBottomSectionHighlightByColor(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.FieldColor != "Green")
        {
            SpriteRenderer spriteToColor = hitInfo.FieldColor == "Red"
                ? spriteToColor = redNumberSprite
                : spriteToColor = blackNumberSprite;

            if (hitInfo.FieldColor == betInfo.FieldColor)
            {
                spriteToColor.color = colorHighlight;
            }
        }
    }

    private void ResetBottomSectionColors()
    {
        firstDozenSprite.color = baseBottomSectionColor;
        secondDozenSprite.color = baseBottomSectionColor;
        thirdDozenSprite.color = baseBottomSectionColor;
        redNumberSprite.color = baseBottomSectionColor;
        blackNumberSprite.color = baseBottomSectionColor;
    }

    private void DisplayColorMultiplier(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.FieldColor == betInfo.FieldColor)
        {
            generalMultiplier = betCalculator.GetEffectiveColorMultiplier(betInfo.FieldColor);
            generalMultiplierText.text = "x" + generalMultiplier.ToString();
        }
    }

    private void DisplayDozenMultiplier(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.Dozen == betInfo.Dozen)
        {
            generalMultiplier = betCalculator.GetEffectiveDozenMultiplier(betInfo.Dozen);
            generalMultiplierText.text = "x" + generalMultiplier.ToString();
        }
    }

    private void DisplayNumberMultiplier(HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.FieldNumber == betInfo.FieldNumber)
        {
            generalMultiplier = betInfo.FieldNumber == 0
                ? betCalculator.GetFieldMultiplier(0, betCalculator.ZeroMultiplier)
                : betCalculator.GetFieldMultiplier(hitInfo.FieldNumber, betCalculator.NumMultiplier);
            generalMultiplierText.text = "x" + generalMultiplier.ToString();
        }
    }
}