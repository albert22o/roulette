using System;
using TMPro;
using UnityEngine;
using Zenject;

public class BetColorsLegendController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText, dozenText, colorText;
    [SerializeField] private TextMeshProUGUI numValue, dozenValue, colorValue;

    [SerializeField] private CurrentBetGUI currentBetDisplay;
    private RoundStateManager roundStateManager;
    private BetCalculator betCalculator;
    private BetTable betTable;
    private TableInfo tableInfo;

    [Inject]
    private void Construct(RoundStateManager roundStateManager, BetCalculator betCalculator, BetTable betTable, TableInfo tableInfo)
    {
        this.roundStateManager = roundStateManager;
        this.betCalculator = betCalculator;
        this.betTable = betTable;
        this.tableInfo = tableInfo;
    }

    void Start()
    {
        roundStateManager.StateChanged += OnRoundStateChanged;
        betCalculator.ColorMultiplierChanged += OnColorMultiplierChanged;
        betCalculator.DozenMultiplierChanged += OnDozenMultiplierChanged;
        betCalculator.NumMultiplierChanged += OnNumMultiplierChanged;
        betTable.ChangeAmountByIndex += OnBetChanged;
         OnNumMultiplierChanged(betCalculator.NumMultiplier);
        OnDozenMultiplierChanged(betCalculator.DozenMultiplier);
        OnColorMultiplierChanged(betCalculator.ColorMultiplier);
        gameObject.SetActive(false);
    }



    private void OnDestroy()
    {
        roundStateManager.StateChanged -= OnRoundStateChanged;
        betCalculator.ColorMultiplierChanged -= OnColorMultiplierChanged;
        betCalculator.DozenMultiplierChanged -= OnDozenMultiplierChanged;
        betCalculator.NumMultiplierChanged -= OnNumMultiplierChanged;
        betTable.ChangeAmountByIndex -= OnBetChanged;
    }

    private void OnBetChanged(int fieldIndex, int betSum)
    {
        if (betSum > 0)
        {
            numValue.text = $"({tableInfo.PocketMapping[fieldIndex].FieldNumber})";
            dozenValue.text = $"({tableInfo.PocketMapping[fieldIndex].Dozen})";
            colorValue.text = $"({tableInfo.PocketMapping[fieldIndex].FieldColor})";
            currentBetDisplay.SetFieldData(tableInfo.PocketMapping[fieldIndex].FieldNumber,
                tableInfo.PocketMapping[fieldIndex].FieldColor, betSum);
            currentBetDisplay.gameObject.SetActive(true); 
        }
        else
        {
            numValue.text = "";
            dozenValue.text = "";
            colorValue.text = "";
            currentBetDisplay.gameObject.SetActive(false);
        }
    }

    private void OnNumMultiplierChanged(int obj)
    {
        numText.text = $"×{obj}";
    }

    private void OnDozenMultiplierChanged(int obj)
    {
        dozenText.text = $"×{obj}";
    }

    private void OnColorMultiplierChanged(int obj)
    {
        colorText.text = $"×{obj}";
    }


    private void OnRoundStateChanged(RoundStateManager.RoundState state)
    {
        gameObject.SetActive(state == RoundStateManager.RoundState.WheelSpining ||
            state == RoundStateManager.RoundState.ChoosingReward);
    }
}
