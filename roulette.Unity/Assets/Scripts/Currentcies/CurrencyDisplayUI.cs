using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CurrencyDisplayUI : MonoBehaviour
{

    [Header("Currency")]
    [SerializeField] private CurrencyCounterUI[] counters;

    [Header("Тестовые значения")]
    [SerializeField] private int[] testAmount;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;


    
    public void ShowTemporarily()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        RefreshAll();
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideAll();
        hideCoroutine = null;
    }

    private void RefreshAll()
    {
        for (int i =0; i < counters.Length; i++)
        {
            int amount = (i < testAmount.Length) ? testAmount[i] : 0;
            counters[i].UpdateAmount(amount);
        }

    }
   
    private void HideAll()
    {
        foreach (var counter in counters)
            counter.ForceHide();
    }



}
