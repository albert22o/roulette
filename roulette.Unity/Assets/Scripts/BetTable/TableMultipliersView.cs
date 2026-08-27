using System;
using System.Linq;
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
    private BetTable betTable;

    [Inject]
    public void Construct(BetCalculator betCalculator, BetTable betTable)
    {
        this.betCalculator = betCalculator;
        this.betTable = betTable;
    }

    private void OnEnable()
    {
        if (betCalculator != null)
        {
            betCalculator.MultipliersUpdated += UpdateUI;
        }

        if (betTable != null)
        {
            betTable.ChangeAmountByIndex += OnBetChanged;
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (betCalculator != null)
        {
            betCalculator.MultipliersUpdated -= UpdateUI;
        }

        if (betTable != null)
        {
            betTable.ChangeAmountByIndex -= OnBetChanged;
        }
    }

    private void OnBetChanged(int index, int amount)
    {
        if (amount > 0)
        {
            UpdateUI();
        }
    }

    public void ForceUpdate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (betCalculator == null || betTable == null) return;

        var activeBets = betTable.NotNullBetAmounts.ToList();

        if (activeBets.Count == 0)
        {
            SetEmptyTexts();
            return;
        }

        var colorBets = activeBets
            .Where(b => b.Item1.FieldColor == ColorType.Red ||
                       b.Item1.FieldColor == ColorType.Black)
            .Select(b => b.Item1.FieldColor)
            .Distinct()
            .ToList();

        if (colorMultiplierText != null)
        {
            if (colorBets.Count > 0)
            {
                var colorTexts = colorBets.Select(c =>
                    $"{c}: x{betCalculator.GetEffectiveColorMultiplier(c)}");
                colorMultiplierText.text = string.Join(", ", colorTexts);
            }
            else
            {
                colorMultiplierText.text = "—";
            }
        }

        var dozenBets = activeBets
            .Where(b => b.Item1.Dozen != 0)
            .Select(b => b.Item1.Dozen)
            .Distinct()
            .ToList();

        if (dozenMultiplierText != null)
        {
            if (dozenBets.Count > 0)
            {
                var dozenTexts = dozenBets.Select(d =>
                    $"{d}: x{betCalculator.GetEffectiveDozenMultiplier(d)}");
                dozenMultiplierText.text = string.Join(", ", dozenTexts);
            }
            else
            {
                dozenMultiplierText.text = "—";
            }
        }

        var numberBets = activeBets
            .Where(b => b.Item1.FieldNumber > 0)
            .Select(b => b.Item1.FieldNumber)
            .Distinct()
            .ToList();

        if (numberMultiplierText != null)
        {
            if (numberBets.Count > 0)
            {
                var numberTexts = numberBets.Select(n =>
                    $"{n}: x{betCalculator.GetFieldMultiplier(n, betCalculator.NumMultiplier)}");
                numberMultiplierText.text = string.Join(", ", numberTexts);
            }
            else
            {
                numberMultiplierText.text = "—";
            }
        }

        var hasZeroBet = activeBets.Any(b => b.Item1.FieldNumber == 0);

        if (zeroMultiplierText != null)
        {
            if (hasZeroBet)
            {
                int multiplier = betCalculator.GetFieldMultiplier(0, betCalculator.ZeroMultiplier);
                zeroMultiplierText.text = $"Zero: x{multiplier}";
            }
            else
            {
                zeroMultiplierText.text = "—";
            }
        }
    }

    private void SetEmptyTexts()
    {
        if (colorMultiplierText != null) colorMultiplierText.text = "—";
        if (dozenMultiplierText != null) dozenMultiplierText.text = "—";
        if (numberMultiplierText != null) numberMultiplierText.text = "—";
        if (zeroMultiplierText != null) zeroMultiplierText.text = "—";
    }
}