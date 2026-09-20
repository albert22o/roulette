using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class ShopInteractionHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineCamera shopCamera;
    [SerializeField] private Shop shop;
    [SerializeField] private FPCInteractionPositionSetter playerPositionSetter;

    private GameStateManager stateManager;
    private bool isInteraction;

    public string Tooltip => "Магазин";

    public bool CanInteract => true;

    [Inject]
    public void Construct(GameStateManager stateManager)
    {
        this.stateManager = stateManager;
        if (stateManager != null)
            stateManager.GameStateChanged += OnGameStateChanged;
    }

    private void StartInteraction()
    {
        if (isInteraction) return;

        isInteraction = true;
        if (shopCamera != null) shopCamera.gameObject.SetActive(true);
        if (shop != null) shop.StartInteraction();
        stateManager.ChangeGameState(GameStateManager.GameState.Shopping);
    }

    private void FinishInteraction()
    {
        if (!isInteraction) return;
        isInteraction = false;
        if (playerPositionSetter != null) playerPositionSetter.ChangePlayerPositionAndRotation();
        if (shopCamera != null) shopCamera.gameObject.SetActive(false);
        if (shop != null) shop.FinishInteraction();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.Shopping && isInteraction)
        {
            FinishInteraction();
        }
    }

    public void Interact()
    {
        StartInteraction();
    }
}
