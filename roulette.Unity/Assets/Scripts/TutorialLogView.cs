using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class TutorialLogView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    private TutorialLog _log;
    private Coroutine _routine;

    [Inject]
    public void Construct(TutorialLog log)
    {
        _log = log;
        _log.LogRequested += OnLogRequested;
        _log.HideRequested += OnHideRequested;
    }

    private void Awake() => _canvasGroup.alpha = 0f;

    private void OnDestroy()
    {
        if (_log == null) return;
        _log.LogRequested -= OnLogRequested;
        _log.HideRequested -= OnHideRequested;
    }

    private void OnLogRequested(string msg, float duration)
    {
        Debug.Log(msg);
        _text.text = msg;
        Restart(ShowRoutine(duration));
    }

    private void OnHideRequested(float fade) => Restart(FadeTo(0f, fade));

    private void Restart(IEnumerator routine)
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(routine);
    }

    private IEnumerator ShowRoutine(float duration)
    {
        yield return FadeTo(1f, TutorialLog.FadeInDuration);
        if (duration > 0f)
        {
            yield return new WaitForSecondsRealtime(duration);
            yield return FadeTo(0f, TutorialLog.FadeOutDuration);
        }
        _routine = null;
    }

    private IEnumerator FadeTo(float target, float time)
    {
        float start = _canvasGroup.alpha;
        for (float t = 0f; t < time; t += Time.unscaledDeltaTime)
        {
            _canvasGroup.alpha = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        _canvasGroup.alpha = target;
    }
}
