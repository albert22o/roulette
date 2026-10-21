using UnityEngine;
using Zenject;

public class TableInteractionHandler : MonoBehaviour, IInteractable
{
    private GameStateManager gameStateManager;
    public string Tooltip => "Стол";

    public bool CanInteract => true;

    [Inject]
    private void Construct(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public void Interact()
    {
        gameStateManager.ChangeGameState(GameStateManager.GameState.BettingCycle);
    }
}
