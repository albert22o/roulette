using UnityEngine;
using TMPro;

public class CurrencyCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel; 
    [SerializeField] private TextMeshProUGUI text;

    [Header("Settings")]
    [SerializeField] private bool alwaysShow = false;

    public void UpdateCounter(int amount)
    {
        UpdateVisibility(amount);
    }

    public void ForceHide()
    {
        panel.SetActive(false);
    }

    private void UpdateVisibility(int amount)
    {
        bool shouldShow = alwaysShow || amount > 0;
        panel.SetActive(shouldShow);

        if (shouldShow)
        {
            text.text = amount.ToString();
        }
    }
}
