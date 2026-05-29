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

    [Inject]
    public void Construct(GameStateManager gameStateManager, PlayerInput playerInput)
    {
        this.gameStateManager = gameStateManager;
        this.playerInput = playerInput;
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
    }

    private void OnInteract(InputAction.CallbackContext _)
    {
        if (!active) return;
        if (!playerInside) return;
        gameStateManager.ChangeGameState(GameStateManager.GameState.BettingСycle);
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