using System;
using UnityEngine;

public class ItemOutline : MonoBehaviour
{
    [SerializeField] private PlaqueItem item;
    [SerializeField] private OutlineController outline;

    private void OnEnable()
    {
        item.HoverEntered += OnHoverEnter;
        item.HoverExited += OnHoverExit;
    }

    private void OnHoverExit(PlaqueItem item)
    {
        outline.ToggleMaterial(false);
    }

    private void OnHoverEnter(PlaqueItem item)
    {
        outline.ToggleMaterial(true);
    }
}
