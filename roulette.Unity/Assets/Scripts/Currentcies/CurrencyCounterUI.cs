using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

using UnityEngine;
using TMPro;

public class CurrencyCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    // Временное поле для теста (потом заменим на реальную валюту)
    [SerializeField] private int mockAmount = 0;

    [Header("Settings")]
    [SerializeField] private bool alwaysShow = false;

    private void OnEnable()
    {
        UpdateVisibility();
    }

    public void RequestShow()
    {
        UpdateVisibility();
    }

    public void ForceHide()
    {
        panel.SetActive(false);
    }

    private void UpdateVisibility()
    {
        bool shouldShow = alwaysShow || mockAmount > 0;
        panel.SetActive(shouldShow);
        if (shouldShow)
            text.text = mockAmount.ToString();
    }
}




/*
public class CurrencyCounterUI : MonoBehaviour
{
    [SerializeField, SerializeReference] private ICurrencyResource resource;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool alwayesShow = false;
    
    
    private void OnEnable()
    {
        if (resource != null)
        {
            resource.AmountChanged += OnAmountChanged;
        }
    }

    private void OnDisable()
    {
        if (resource != null)
            resource.AmountChanged -= OnAmountChanged;
    }

    public void RequestShow()
    {
        UpdateVisibility();
    }
    public void ForceHide()
    {
        panel.SetActive(false);
    }
    public void OnAmountChanged(int newAmount)
    {
        UpdateVisibility();
    }
    public void UpdateVisibility()
    {
        if (resource == null) return;
        int amount = resource.Amount;
        bool shouldShow = alwayesShow || amount > 0;

        panel.SetActive(shouldShow);
        if (shouldShow)
            text.text = amount.ToString();
    }
}*/
