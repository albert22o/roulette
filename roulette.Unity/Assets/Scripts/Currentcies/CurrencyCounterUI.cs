using TMPro;
using UnityEngine;

public class CurrencyCounterUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool alwayesShow = false;

    public void UpdateAmount (int amount)
    {
        bool shouldShow = alwayesShow || amount > 0;
        panel.SetActive(shouldShow);

        if (shouldShow)
            text.text = amount.ToString();
    }
    public void ForceHide()
    {
        panel.SetActive(false);
    }
}
