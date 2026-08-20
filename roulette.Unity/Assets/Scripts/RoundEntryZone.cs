using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(Collider))]
public class RoundEntryZone : MonoBehaviour
{
    private bool active;
    private bool playerInside;
    private GameStateManager gameStateManager;
    private PlayerInput playerInput;
    [SerializeField] private GameStateManager.GameState state;

    [Inject]
    public void Construct(GameStateManager gameStateManager, PlayerInput playerInput)
    {
        this.gameStateManager = gameStateManager;
        this.playerInput = playerInput;
        this.gameStateManager.GameStateChanged += OnGameStateChanged;
    }

    public void Start()
    {
        playerInput.actions["Interact"].performed += OnInteract;
        active = true;
    }

    private void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Interact"].performed -= OnInteract;

        if (gameStateManager != null)
            gameStateManager.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        if (newState != GameStateManager.GameState.FreeRoam)
        {
            playerInside = false;
        }
    }

    private void OnInteract(InputAction.CallbackContext _)
    {
        if (!active) return;
        if (!playerInside) return;
        gameStateManager.ChangeGameState(state);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}