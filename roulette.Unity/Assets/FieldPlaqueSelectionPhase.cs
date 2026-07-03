using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldPlaqueSelectionPhase : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BetFieldVisual staticField, animatedField;
    [SerializeField] private Animator animator;

    private event Action Clicked;
    private BetInfo betInfo;

    private int animationHash = Animator.StringToHash("ChangeField");



    public void Init(BetInfo betInfo, Action clickCallback)
    {
        this.betInfo = betInfo;
        UpdateField(true);

        Clicked = clickCallback;
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
            animatedField.gameObject.SetActive(false);
        }
        else
        {
            animator.SetTrigger(animationHash);
            animatedField.SetColor(betInfo.FieldColor);
            animatedField.SetNumber(betInfo.FieldNumber);
            animatedField.gameObject.SetActive(true);
        }
    }

    public void Highlight(bool value) => staticField.Highlight(value);

    public void OnAnimationFinish()
    {
        UpdateField(true);
        staticField.Highlight(false);
        animatedField.gameObject.SetActive(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke();
    }
}
