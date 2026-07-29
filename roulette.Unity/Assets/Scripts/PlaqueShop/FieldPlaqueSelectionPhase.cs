using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldPlaqueSelectionPhase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private BetFieldVisual staticField, animatedField;
    [SerializeField] private Animator animator;
    [SerializeField] private OutlineController outline;

    public event Action<FieldPlaqueSelectionPhase> Clicked;
    public event Action<FieldPlaqueSelectionPhase> HoverEnter, HoverExit;
    public int Index { get; private set; }
    private BetInfo betInfo;

    private int animationHash = Animator.StringToHash("ChangeField");



    public void Init(BetInfo betInfo, int index)
    {
        this.betInfo = betInfo;
        this.Index = index;
        UpdateField(true);
    }

    private void OnDestroy()
    {
        if (Clicked != null)
            Clicked -= Clicked;
    }

    public void UpdateField(bool updateImmediately)
    {
        if (staticField.Color == betInfo.FieldColor && staticField.Number == betInfo.FieldNumber)
            return;
        if (updateImmediately)
        {
            staticField.SetColor(betInfo.FieldColor);
            staticField.SetNumber(betInfo.FieldNumber);
        }
        else
        {
            animator.SetTrigger(animationHash);
            animatedField.SetColor(betInfo.FieldColor);
            animatedField.SetNumber(betInfo.FieldNumber);
        }
    }

    public void Highlight(bool value) => staticField.Highlight(value);

    public void OnAnimationFinish()
    {
        UpdateField(true);
        staticField.Highlight(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverExit?.Invoke(this);
    }

    public void ToggleOutline(bool value) => outline.ToggleMaterial(value);
}
