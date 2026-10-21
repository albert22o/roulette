using UnityEngine;
using Zenject;

public class TutorialLegendTarget : MonoBehaviour
{
    [SerializeField] private TutorialTargetId _id = TutorialTargetId.Multipliers;
    [SerializeField] private BetColorsLegendController _legend;
    [SerializeField] private GameObject _frame; // рамка внутри легенды, опционально

    private TutorialHighlight _highlight;

    [Inject]
    public void Construct(TutorialHighlight highlight)
    {
        _highlight = highlight;
        _highlight.HighlightRequested += OnHighlightRequested;
    }

    private void OnDestroy()
    {
        if (_highlight != null)
            _highlight.HighlightRequested -= OnHighlightRequested;
    }

    private void OnHighlightRequested(TutorialTargetId id, bool enabled, bool blink)
    {
        if (id != _id) return;

        _legend.SetForcedVisible(enabled);
        if (_frame != null) _frame.SetActive(enabled);
    }
}