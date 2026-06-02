using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CurrencyDisplayUI : MonoBehaviour
{
    
    [Header("Currency")]

    [SerializeField] private GameObject goldchipPanel;
    [SerializeField] private TextMeshProUGUI goldchipText;

    [SerializeField] private GameObject bosschipPanel;
    [SerializeField] private TextMeshProUGUI bosschipText;

    [SerializeField] private GameObject ticketPanel;
    [SerializeField] private TextMeshProUGUI ticketText;

    [SerializeField] private GameObject moneyPanel;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Тестовые значения")]
    [SerializeField] private int testdataGoldChipAmount = 0;
    [SerializeField] private int testdataBossChipAmount = 0;
    [SerializeField] private int testdataTicketAmount = 0;
    [SerializeField] private int testdataMoneyAmount = 0;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;


    
    public void ShowTemporarily()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        RefreshAllCurrencies();
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideAllCurrencies();
        hideCoroutine = null;
    }

    private void RefreshAllCurrencies()
    {
        UpdateSingleCurrency(goldchipPanel, goldchipText, testdataGoldChipAmount, true);
        UpdateSingleCurrency(bosschipPanel, bosschipText, testdataBossChipAmount, false);
        UpdateSingleCurrency(ticketPanel, ticketText, testdataTicketAmount, false);
        UpdateSingleCurrency(moneyPanel, moneyText, testdataMoneyAmount, true);

    }
    private void UpdateSingleCurrency(GameObject panel, TextMeshProUGUI text, int amount, bool alwayesShow)
    {
        bool shouldShow = alwayesShow || amount > 0;
        panel.SetActive(shouldShow);
        text.text = amount.ToString();
    }
    private void HideAllCurrencies()
    {
        goldchipPanel.SetActive(false);
        bosschipPanel.SetActive(false);
        ticketPanel.SetActive(false);
        moneyPanel.SetActive(false);
    }



}
