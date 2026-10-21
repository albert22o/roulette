using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TutorialHighlightTarget : MonoBehaviour
{
    [SerializeField] private TutorialTargetId _id;

    [Header("3D")]
    [SerializeField] private OutlineController _outline;

    [Header("UI: компоненты Outline/Shadow (выключены по умолчанию)")]
    [SerializeField] private Behaviour[] _uiEffects;

    [Header("UI: подкраска")]
    [SerializeField] private Graphic[] _tintGraphics;
    [SerializeField] private Color _tintColor = new Color(1f, 0.85f, 0.2f);

    [Header("Мигание")]
    [SerializeField] private float _blinkInterval = 0.5f;

    private TutorialHighlight _highlight;
    private Coroutine _blinkRoutine;
    private Coroutine _pulseRoutine;
    private Vector3 _baseScale;
    private Color[] _baseColors;

    [Inject]
    public void Construct(TutorialHighlight highlight)
    {
        _highlight = highlight;
        _highlight.HighlightRequested += OnHighlightRequested;
    }

    private void Awake()
    {
        if (_tintGraphics != null)
        {
            _baseColors = new Color[_tintGraphics.Length];
            for (int i = 0; i < _tintGraphics.Length; i++)
                _baseColors[i] = _tintGraphics[i].color;
        }
    }

    private void OnDestroy()
    {
        if (_highlight != null)
            _highlight.HighlightRequested -= OnHighlightRequested;
    }

    private void OnDisable()
    {
        StopRoutines();
        Apply(false);
    }

    private void OnHighlightRequested(TutorialTargetId id, bool enabled, bool blink)
    {
        if (id != _id) return;

        StopRoutines();
        Apply(enabled);

        if (!enabled || !isActiveAndEnabled) return;

        if (blink)
            _blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    private void StopRoutines()
    {
        if (_blinkRoutine != null) StopCoroutine(_blinkRoutine);
        if (_pulseRoutine != null) StopCoroutine(_pulseRoutine);
        _blinkRoutine = null;
        _pulseRoutine = null;
    }

    private IEnumerator BlinkRoutine()
    {
        bool on = true;
        while (true)
        {
            yield return new WaitForSecondsRealtime(_blinkInterval);
            on = !on;
            Apply(on);
        }
    }

    private void Apply(bool on)
    {
        if (_outline != null)
            _outline.ToggleMaterial(on);

        if (_uiEffects != null)
            foreach (var e in _uiEffects)
                if (e != null) e.enabled = on;

        if (_tintGraphics != null)
            for (int i = 0; i < _tintGraphics.Length; i++)
                if (_tintGraphics[i] != null)
                    _tintGraphics[i].color = on ? _tintColor : _baseColors[i];
    }
}