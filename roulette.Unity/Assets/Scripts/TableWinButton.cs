using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TableWinButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action Clicked, HoverEnter, HoverExit;
    [SerializeField] private OutlineController outlineController;
    [SerializeField] private ButtonAnimation3D buttonAnimation;
    private RoundManager roundManager;
    private RoundStateManager roundStateManager;
    private bool isHovered;

    [Inject]
    public void Construct(RoundManager roundManager, RoundStateManager roundStateManager)
    {
        this.roundManager = roundManager;
        this.roundStateManager = roundStateManager;
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        if (roundManager.AbleToWin)
        {
            if (isHovered)
            {
                HoverEnter?.Invoke();
                outlineController.ToggleMaterial(true);
            }
        }
        else if (isHovered == true)
        {
            HoverExit?.Invoke();
            outlineController.ToggleMaterial(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (roundManager.AbleToWin)
        {
            Clicked?.Invoke();
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(false);
            buttonAnimation.Press();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (roundManager.AbleToWin)
        {
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (roundManager.AbleToWin)
        {
            HoverExit?.Invoke();
            outlineController.ToggleMaterial(false);
        }
    }
}
