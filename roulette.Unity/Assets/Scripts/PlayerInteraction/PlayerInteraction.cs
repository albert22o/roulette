using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InteractionTooltip tooltip;
    [SerializeField] private InputActionReference interactAction;

    private GameStateManager gameStateManager;

    private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentTarget;

    [Inject]
    private void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    private void Start()
    {
        gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (gameStateManager != null)
        {
            gameStateManager.GameStateChanged -= OnGameStateChanged;
        }
    }


    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.FreeRoam)
        {
            gameObject.SetActive(true);
            if (interactAction != null)
            {
                interactAction.action.performed += OnInteractPerformed;
            }
        }
        else
        {
            gameObject.SetActive(false);
            if (interactAction != null)
            {
                interactAction.action.performed -= OnInteractPerformed;
            }

            nearbyInteractables.Clear();
            currentTarget = null;
            if (tooltip != null)
                tooltip.Hide();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        var target = currentTarget;
        if (target == null)
            return;

        target.Interact();

        if (this == null || !isActiveAndEnabled)
            return;

        if (ReferenceEquals(target, currentTarget) && !target.CanInteract)
        {
            UpdateCurrentTarget(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);

            if (ReferenceEquals(interactable, currentTarget))
            {
                UpdateCurrentTarget(null);
            }
        }
    }


    private void Update()
    {
        IInteractable best = null;
        float bestDist = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = nearbyInteractables.Count - 1; i >= 0; i--)
        {
            var interactable = nearbyInteractables[i];
            var mb = interactable as MonoBehaviour;

            if (mb == null)
            {
                nearbyInteractables.RemoveAt(i);
                continue;
            }

            if (!interactable.CanInteract)
                continue;

            float dist = (mb.transform.position - myPos).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
            }
        }

        UpdateCurrentTarget(best);
    }

    private void UpdateCurrentTarget(IInteractable newTarget)
    {
        currentTarget = newTarget;

        if (tooltip == null)
            return;

        if (currentTarget != null)
            tooltip.ShowTooltip(currentTarget.Tooltip);
        else
            tooltip.Hide();
    }
}
