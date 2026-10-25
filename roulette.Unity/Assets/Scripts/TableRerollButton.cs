using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;
using Zenject;

public class TableRerollButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action Clicked, HoverEnter, HoverExit;
    [SerializeField] private OutlineController outlineController;
    [SerializeField] private ButtonAnimation3D buttonAnimation;
    [SerializeField] private VisualEffect vfx;

    private RoundManager roundManager;
    private RoundStateManager roundStateManager;
    private bool isHovered;
    private SFXManager sfxManager;

    [Inject]
    private void Construct(RoundManager roundManager, RoundStateManager roundStateManager, SFXManager sfxManager)
    {
        this.roundManager = roundManager;
        this.roundStateManager = roundStateManager; 
        this.sfxManager = sfxManager;
        roundStateManager.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(RoundStateManager.RoundState state)
    {
        if (roundManager.CanReroll)
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
        if (roundManager.CanReroll)
        {

            Clicked?.Invoke();
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(false);
            buttonAnimation.Press();
            vfx.SendEvent("Explode");
            sfxManager.Play("ButtonClick");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (roundManager.CanReroll)
        {
            HoverEnter?.Invoke();
            outlineController.ToggleMaterial(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (roundManager.CanReroll)
        {
            HoverExit?.Invoke();
            outlineController.ToggleMaterial(false);
        }
    }
}
