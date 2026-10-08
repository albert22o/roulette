using System;
using UnityEngine;

public class RouletteOutline : MonoBehaviour
{
    [SerializeField] private RouletteClickHandler clickHandler;
    [SerializeField] private OutlineController outline;

    private void Start()
    {
        clickHandler.HoverEnter += OnHoverEnter;
        clickHandler.HoverExit += OnHoverExit;
    }

    private void OnDestroy()
    {
        clickHandler.HoverEnter -= OnHoverEnter;
        clickHandler.HoverExit -= OnHoverExit;
    }

    private void OnHoverExit()
    {
        outline.ToggleMaterial(false);
    }

    private void OnHoverEnter()
    {
        outline.ToggleMaterial(true);
    }
}
