using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CancelInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference cancelActionReference;

    private GameStateManager gameStateManager;

    [Inject]
    public void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    private void OnEnable()
    {
        if (cancelActionReference != null)
        {
            cancelActionReference.action.Enable();
            cancelActionReference.action.performed += OnCancelPressed;
        }
    }

    private void OnDisable()
    {
        if (cancelActionReference != null)
        {
            cancelActionReference.action.performed -= OnCancelPressed;
            cancelActionReference.action.Disable();
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        gameStateManager.ChangeGameState(GameStateManager.GameState.FreeRoam);
    }
}
