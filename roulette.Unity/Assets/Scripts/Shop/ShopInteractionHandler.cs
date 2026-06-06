using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class ShopInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camera;
    [SerializeField] private Shop shop;
    private GameStateManager stateManager;
    private bool isInteraction;

    [Inject]
    public void Construct(Money money, GoldenChips goldenChips, Inventory inventory, GameStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    private void OnEnable()
    {
        stateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        stateManager.GameStateChanged -= OnGameStateChanged;
    }

    public void StartInteraction()
    {
        camera.gameObject.SetActive(true);
        isInteraction = true;
        shop.StartInteraction();
    }

    public void FinishInteraction()
    {
        camera.gameObject.SetActive(false);
        isInteraction = false;
        shop.FinishInteraction();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Shopping)
        {
            StartInteraction();
        }
        else if (isInteraction == true)
        {
            FinishInteraction();
        }
    }
}
