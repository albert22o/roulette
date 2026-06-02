using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CurrencyDisplayUI : MonoBehaviour
{

    [Header("Currency")]
    [SerializeField] private CurrencyCounterUI[] counters;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    private Coroutine hideCoroutine;


    
    public void ShowTemporarily()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        foreach (var counter in counters)
            counter.RequestShow();

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        
        foreach (var counter in counters)
            counter.ForceHide();

        hideCoroutine = null;
    }




}
