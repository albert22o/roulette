using System;
using UnityEngine;

public class PlaqueItem : MonoBehaviour, IMouseInteractable, IHoverable, IClickable
{
    [SerializeField] private ItemPlace itemPlace;
    public event Action<PlaqueItem> HoverEntered, HoverExited, Clicked;
    public PlaqueData Data { get; private set; }

    public bool IsInteractable => Data != null;

    public void OnClick() => Clicked?.Invoke(this);

    public void OnHoverEnter() => HoverEntered?.Invoke(this);

    public void OnHoverExit() => HoverExited?.Invoke(this);

    public void SetData(PlaqueData data)
    {
        Data = data;
        itemPlace.SetItem(data);
    }
}
