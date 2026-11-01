using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class TutorialLogView : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform bettingLayout;   
    [SerializeField] private RectTransform defaultLayout;
    [SerializeField] private RectTransform rectTransform;

    private TutorialLog log;
    private Coroutine routine;

    [Inject]
    public void Construct(TutorialLog log, GameStateManager gameStateManager)
    {
        gameStateManager.GameStateChanged += OnStateChanged;
        this.log = log;
        this.log.LogRequested += OnLogRequested;
        this.log.HideRequested += OnHideRequested;
    }

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    private void OnStateChanged(GameStateManager.GameState state)
    {
        ApplyLayout(state == GameStateManager.GameState.BettingCycle
            ? bettingLayout
            : defaultLayout);
    }

    private void ApplyLayout(RectTransform src)
    {
        rectTransform.anchorMin = src.anchorMin;
        rectTransform.anchorMax = src.anchorMax;
        rectTransform.pivot = src.pivot;
        rectTransform.anchoredPosition = src.anchoredPosition;
        rectTransform.sizeDelta = src.sizeDelta;
    }

    private void OnDestroy()
    {
        if (log == null) return;
        log.LogRequested -= OnLogRequested;
        log.HideRequested -= OnHideRequested;
    }

    private void OnLogRequested(string msg, float duration)
    {
        Debug.Log(msg);
        text.text = msg;
        Restart(ShowRoutine(duration));
    }

    private void OnHideRequested(float fade) => Restart(FadeTo(0f, fade));

    private void Restart(IEnumerator routine)
    {
        if (this.routine != null) StopCoroutine(this.routine);
        this.routine = StartCoroutine(routine);
    }

    private IEnumerator ShowRoutine(float duration)
    {
        yield return FadeTo(1f, TutorialLog.FadeInDuration);
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            yield return FadeTo(0f, TutorialLog.FadeOutDuration);
        }
        routine = null;
    }

    private IEnumerator FadeTo(float target, float time)
    {
        float start = canvasGroup.alpha;
        for (float t = 0f; t < time; t += Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        canvasGroup.alpha = target;
    }
}
