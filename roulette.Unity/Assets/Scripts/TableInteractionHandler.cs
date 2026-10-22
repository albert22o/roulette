using UnityEngine;
using Zenject;

public class TableInteractionHandler : MonoBehaviour, IInteractable
{
    private GameStateManager gameStateManager;
    public string Tooltip => Loc.Get("interact.table");

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
