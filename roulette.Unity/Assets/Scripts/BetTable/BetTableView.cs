using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

class BetTableView : MonoBehaviour
{
    [SerializeField] List<BetButton> betButtons = new();

    private BetTable betTable;
    private TableConfig tableConfig;

    [Inject]
    public void Construct(BetTable betTable, TableConfig tableConfig)
    {
        this.betTable = betTable;
        this.tableConfig = tableConfig;
    }

    public void Init()
    {
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
}