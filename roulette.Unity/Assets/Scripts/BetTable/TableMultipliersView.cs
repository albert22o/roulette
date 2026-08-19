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

        int currentColor = betCalculator.GetEffectiveColorMultiplier(ColorType.Red);
        int currentDozen = betCalculator.GetEffectiveDozenMultiplier(DozenType.First);
        int currentNumber = betCalculator.GetFieldMultiplier(1, betCalculator.NumMultiplier);
        int currentZero = betCalculator.GetFieldMultiplier(0, betCalculator.ZeroMultiplier);

        Debug.Log($"<color=cyan>[TableMultipliersView]</color> " +
                  $"Zero: x{currentZero}, Number(1): x{currentNumber}, Dozen(1st): x{currentDozen}, Color(Red): x{currentColor}");

        if (colorMultiplierText != null)
            colorMultiplierText.text = $"Color (Red): x{currentColor}";

        if (dozenMultiplierText != null)
            dozenMultiplierText.text = $"Dozen (1st): x{currentDozen}";

        if (numberMultiplierText != null)
            numberMultiplierText.text = $"Number (1): x{currentNumber}";

        if (zeroMultiplierText != null)
            zeroMultiplierText.text = $"Zero: x{currentZero}";
    }
}