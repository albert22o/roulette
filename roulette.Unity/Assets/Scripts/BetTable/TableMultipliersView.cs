using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

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

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnBetChanged(int index, int amount)
    {
        UpdateUI();
    }

    private void OnLocaleChanged(Locale locale)
    {
        UpdateUI();
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
            ShowGeneralMultipliers();
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
                    Loc.Get("multipliers.color_bet", Loc.Color(c), betCalculator.GetEffectiveColorMultiplier(c)));
                colorMultiplierText.text = string.Join(", ", colorTexts);
            }
            else
            {
                colorMultiplierText.text = "";
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
                    Loc.Get("multipliers.dozen_bet", (int)d,betCalculator.GetEffectiveDozenMultiplier(d)));
                dozenMultiplierText.text = string.Join(", ", dozenTexts);
            }
            else
            {
                dozenMultiplierText.text = "";
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
                    $"{n}: ×{betCalculator.GetFieldMultiplier(n, betCalculator.NumMultiplier)}");
                numberMultiplierText.text = string.Join(", ", numberTexts);
            }
            else
            {
                numberMultiplierText.text = "";
            }
        }

        var hasZeroBet = activeBets.Any(b => b.Item1.FieldNumber == 0);

        if (zeroMultiplierText != null)
        {
            if (hasZeroBet)
            {
                zeroMultiplierText.text = Loc.Get("multipliers.zero", betCalculator.ZeroMultiplier);
            }
            else
            {
                zeroMultiplierText.text = "";
            }
        }
    }

    private void ShowGeneralMultipliers()
    {
        if (colorMultiplierText != null) colorMultiplierText.text = Loc.Get("multipliers.color", betCalculator.ColorMultiplier);
        if (dozenMultiplierText != null) dozenMultiplierText.text = Loc.Get("multipliers.dozen", betCalculator.DozenMultiplier);
        if (numberMultiplierText != null) numberMultiplierText.text = Loc.Get("multipliers.number", betCalculator.NumMultiplier);
        if (zeroMultiplierText != null) zeroMultiplierText.text = Loc.Get("multipliers.zero", betCalculator.ZeroMultiplier);
    }
}