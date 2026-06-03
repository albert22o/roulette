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

    private BetTable betTable;
    private TableConfig tableConfig;
    private RoundStateManager roundStateManager;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig, BetCalculator betCalculator, RoundStateManager roundStateManager)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
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

    private void OnHighlightButton(HitInfo hitInfo)
    {
        StartCoroutine(ResultDisplaySequence(hitInfo));
        Debug.Log("On highlight");
    }

    public IEnumerator ResultDisplaySequence(HitInfo hitInfo)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("corroutine");
        //Для каждой ненулевой ставки
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            Debug.Log("ЕЕЕ БОЙ");
            //Если цвет ставки совпадает с выпавшим
            if (hitInfo.FieldColor == betInfo.FieldColor)
            {
                Debug.Log("color");
                //Каждая кнопка этого цвета подсвечивается
                foreach (BetButton button in betButtons)
                {
                    if (button.Color == betInfo.FieldColor)
                        button.Highlight(true);
                }
            }
            //Если цвет ставки не совпадает
            else
            {
                //Затемняется
            }

            yield return new WaitForSeconds(1f);

            //Если дюжина ставки совпадает с выпавшей
            if (hitInfo.Dozen == betInfo.Dozen)
            {
                //Каждая кнопка этой дюжины подсвечивается
                foreach (BetButton button in betButtons)
                {
                    if (button.GetDozen() == betInfo.Dozen)
                        button.Highlight(true);
                }
            }
            //Если дюжина ставки не совпадает
            else
            {
                //Затемняется
            }

            yield return new WaitForSeconds(1f);

            if (hitInfo.FieldNumber == betInfo.FieldNumber)
            {
                //Каждая кнопка этой дюжины подсвечивается
                foreach (BetButton button in betButtons)
                {
                    if (button.FieldNum == betInfo.FieldNumber)
                        button.Highlight(true);
                }
            }
        }

        yield return new WaitForSeconds(2f);

        foreach (BetButton button in betButtons)
                button.Highlight(false);

        betTable.ClearBets();
    }
}