using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using Zenject;

public class CancelInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference cancelActionReference;
    [SerializeField] private InteractionTooltip tooltip;

    private GameStateManager gameStateManager;
    private int lockCount = 0;
    private bool isActionSubscribed = false;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        LocalizationSettings.SelectedLocaleChanged += (_) => UpdateTooltipState();
    }

    public void LockCancel()
    {
        lockCount++;
        EvaluateState();
    }

    public void UnlockCancel()
    {
        lockCount--;
        if (lockCount < 0)
        {
            Debug.LogError("lockCount shouldn't be < 0");
        }
        EvaluateState();
    }

    private void EvaluateState()
    {
        Debug.Log(lockCount);
        bool shouldBeActive = lockCount == 0;

        if (shouldBeActive && !isActionSubscribed)
        {
            isActionSubscribed = true;
            cancelActionReference.action.performed += OnCancelPressed;
            UpdateTooltipState();
        }
        else if (!shouldBeActive && isActionSubscribed)
        {
            isActionSubscribed = false;
            tooltip.Hide();
            cancelActionReference.action.performed -= OnCancelPressed;
        }
    }

    private void UpdateTooltipState()
    {
        if (isActionSubscribed)
        {
            tooltip.ShowTooltip(Loc.Get("playerInteraction.exit"));
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
    }
}
