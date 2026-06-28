using System;
using UnityEngine;

public class PlaqueVendingMachine : MonoBehaviour, IMouseInteractable, IClickable
{
    public event Action Clicked;
    public bool IsInteractable => true;

    public void OnClick() => Clicked?.Invoke();
}
