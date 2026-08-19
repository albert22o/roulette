using System;
using TMPro;
using UnityEngine;
using Zenject;

public class TableMultipliersView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI colorMultiplierText;
    [SerializeField] private TextMeshProUGUI dozenMultiplierText;
    [SerializeField] private TextMeshProUGUI numberMultiplierText;
    [SerializeField] private TextMeshProUGUI zeroMultiplierText;

    private BetCalculator betCalculator;

    [Inject]
    public void Construct(BetCalculator betCalculator)
    {
        this.betCalculator = betCalculator;
    }

    private void OnEnable()
    {
        if (betCalculator != null)
        {
            betCalculator.MultipliersUpdated += UpdateUI;
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (betCalculator != null)
        {
            betCalculator.MultipliersUpdated -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (betCalculator == null) return;

        Debug.Log($"<color=cyan>[TableMultipliersView]</color> Получено обновление! Текущие значения: " +
                  $"Zero: x{betCalculator.ZeroMultiplier}, " +
                  $"Number: x{betCalculator.NumMultiplier}, " +
                  $"Dozen: x{betCalculator.DozenMultiplier}, " +
                  $"Color: x{betCalculator.ColorMultiplier}");

        if (colorMultiplierText != null)
            colorMultiplierText.text = $"Color: x{betCalculator.ColorMultiplier}";

        if (dozenMultiplierText != null)
            dozenMultiplierText.text = $"Dozen: x{betCalculator.DozenMultiplier}";

        if (numberMultiplierText != null)
            numberMultiplierText.text = $"Number: x{betCalculator.NumMultiplier}";

        if (zeroMultiplierText != null)
            zeroMultiplierText.text = $"Zero: x{betCalculator.ZeroMultiplier}";
    }
}