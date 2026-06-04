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

        yield return new WaitForSeconds(3f);

        //Для каждой ненулевой ставки
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            foreach (BetButton button in betButtons)
            {
                ProcessButtonHighlightByColor(button, hitInfo, betInfo);
            }
            ProcessBottomSectionHighlightByColor(hitInfo, betInfo);

            yield return new WaitForSeconds(1f);

            foreach (BetButton button in betButtons)
            {
                ProcessButtonHighlightByDozen(button, hitInfo, betInfo);
            }
            ProcessBottomSectionHighlightByDozen(hitInfo, betInfo);

            yield return new WaitForSeconds(1f);

            foreach (BetButton button in betButtons)
            {
                ProcessButtonHighlightByNumber(button, hitInfo, betInfo);
            }
        }

        yield return new WaitForSeconds(2f);

        foreach (BetButton button in betButtons)
            button.Highlight(false, BetButton.HighlightPhase.None);
        ResetBottomSectionColors();

        betCalculator.GetRewardAction();
        betTable.ClearBets();
    }

    private void ProcessButtonHighlightByColor(BetButton button, HitInfo hitInfo, BetInfo betInfo)
    {
        //Если цвет ставки совпадает с выпавшим
        if (hitInfo.FieldColor == betInfo.FieldColor)
        {
            //Кнопка этого цвета подсвечивается
            if (button.Color == betInfo.FieldColor)
                button.Highlight(true, BetButton.HighlightPhase.Color);
        }
        //Если цвет ставки не совпадает
        else
        {
            if (button.Color == betInfo.FieldColor)
                button.Lowlight(true);
        }
    }

    private void ProcessButtonHighlightByDozen(BetButton button, HitInfo hitInfo, BetInfo betInfo)
    {
        //Если дюжина ставки совпадает с выпавшей
        if (hitInfo.Dozen == betInfo.Dozen)
        {
            //Каждая кнопка этой дюжины подсвечивается
            if (button.Dozen == betInfo.Dozen)
                button.Highlight(true, BetButton.HighlightPhase.Dozen);
        }
        //Если дюжина ставки не совпадает
        else
        {
            //Затемняется
            if (button.Dozen == hitInfo.Dozen)
                button.Lowlight(true);
        }
    }

    private void ProcessButtonHighlightByNumber(BetButton button, HitInfo hitInfo, BetInfo betInfo)
    {
        if (hitInfo.FieldNumber == betInfo.FieldNumber)
        {
            //Кнопка этой цифры подсвечивается
            if (button.FieldNum == betInfo.FieldNumber)
                button.Highlight(true, BetButton.HighlightPhase.Number);
        }
        //Если цифра ставки не совпадает
        else
        {
            //Затемняется
            if (button.FieldNum == hitInfo.FieldNumber)
                button.Lowlight(true);
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
            else
                spriteToColor.color = lowlight;
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
            else
            {
                spriteToColor.color = lowlight;
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
}