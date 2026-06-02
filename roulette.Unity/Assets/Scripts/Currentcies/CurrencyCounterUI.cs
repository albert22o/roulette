using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
}
