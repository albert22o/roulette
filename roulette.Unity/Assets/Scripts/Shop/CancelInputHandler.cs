using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using Zenject;

public class CancelInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference cancelActionReference;
    [SerializeField] private InteractionTooltip tooltip;

    private GameStateManager gameStateManager;
    private bool isCancelAvailable = false;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
        LocalizationSettings.SelectedLocaleChanged += (_) => Toggle(isCancelAvailable);
    }

    public void Toggle(bool isCancelAvailable)
    {
        this.isCancelAvailable = isCancelAvailable;
        if (isCancelAvailable)
        {
            cancelActionReference.action.performed += OnCancelPressed;
            tooltip.ShowTooltip(Loc.Get("playerInteraction.exit"));
        }
        else
        {
            tooltip.Hide();
            cancelActionReference.action.performed -= OnCancelPressed;
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
    }
}
