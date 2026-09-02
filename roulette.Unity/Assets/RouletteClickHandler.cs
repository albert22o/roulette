using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class RouletteClickHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MeshCollider meshCollider;
    private bool isInteractable = false;
    public bool IsInteractable
    {
        get { return isInteractable; }
        set
        {
            isInteractable = value;
            meshCollider.enabled = value;
        }
    }
    public event Action Clicked, HoverEnter, HoverExit;
    public BetTable betTable;

    [Inject]
    private void Costruct(BetTable betTable)
    {
        this.betTable = betTable;
    }

    private void Start()
    {
        betTable.HasActiveBetsChanged += OnHasActiveBetsChanged;
    }

    private void OnDestroy()
    {
        betTable.HasActiveBetsChanged -= OnHasActiveBetsChanged;
    }

    private void OnHasActiveBetsChanged(bool hasBet)
    {
        IsInteractable = hasBet;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        Clicked?.Invoke();
        HoverExit?.Invoke();
        IsInteractable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        HoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        HoverExit?.Invoke();
    }
}
