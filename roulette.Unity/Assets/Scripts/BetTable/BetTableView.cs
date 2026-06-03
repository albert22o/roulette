using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using Zenject;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

class BetTableView : MonoBehaviour
{
    [SerializeField] TextMeshPro availableSumText;
    [SerializeField] List<BetButton> betButtons = new();

    private BetTable betTable;
    private TableConfig tableConfig;
    private RoundStateManager roundStateManager;
    public event Action<int> GetReward;

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

    private void OnHighlightButton(HitInfo hitInfo, int reward)
    {
        StartCoroutine(ResultDisplaySequence(hitInfo, reward));
    }

    public IEnumerator ResultDisplaySequence(HitInfo hitInfo, int reward)
    {
        roundStateManager.ChangeState(RoundStateManager.RoundState.BetResults);

        yield return new WaitForSeconds(3f);

        //Для каждой ненулевой ставки
        foreach ((var betInfo, var amount) in betTable.NotNullBetAmounts)
        {
            foreach (BetButton button in betButtons)
            {
                //Если цвет ставки совпадает с выпавшим
                if (hitInfo.FieldColor == betInfo.FieldColor)
                {
                    //Каждая кнопка этого цвета подсвечивается
                    if (button.Color == betInfo.FieldColor)
                        button.Highlight(false, true, 1);
                }
                //Если цвет ставки не совпадает
                else
                {
                    if (button.Color == betInfo.FieldColor)
                        button.Highlight(true, true, 0);
                }
            }

            yield return new WaitForSeconds(1f);

            foreach (BetButton button in betButtons)
            {
                //Если дюжина ставки совпадает с выпавшей
                if (hitInfo.Dozen == betInfo.Dozen)
                {
                    //Каждая кнопка этой дюжины подсвечивается
                    if (button.GetDozen() == betInfo.Dozen)
                        button.Highlight(false, true, 2);
                }
                //Если дюжина ставки не совпадает
                else
                {
                    //Затемняется
                    if (button.GetDozen() == hitInfo.Dozen)
                        button.Highlight(true, true, 0);
                }
            }

            yield return new WaitForSeconds(1f);

            foreach (BetButton button in betButtons)
            {
                if (hitInfo.FieldNumber == betInfo.FieldNumber)
                {
                    //Кнопка этой цифры подсвечивается
                    if (button.FieldNum == betInfo.FieldNumber)
                        button.Highlight(false, true, 3);
                }
                //Если цифра ставки не совпадает
                else
                {
                    //Затемняется
                    if (button.FieldNum == hitInfo.FieldNumber)
                        button.Highlight(true, true, 0);
                }
            }
        }

        yield return new WaitForSeconds(2f);

        foreach (BetButton button in betButtons)
                button.Highlight(false, false, 0);

        GetReward?.Invoke(reward);
        betTable.ClearBets();
    }
}