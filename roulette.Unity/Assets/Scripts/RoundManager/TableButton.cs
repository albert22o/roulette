using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class TableButton : MonoBehaviour, IPointerClickHandler
{
    public event Action Clicked;
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Clicked?.Invoke();
    }

    // Временно повесь на кнопку
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}