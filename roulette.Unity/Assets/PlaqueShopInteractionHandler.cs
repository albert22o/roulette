using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class PlaqueShopInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camera;
    [SerializeField] private PlaqueShopView shop;

    private GameStateManager stateManager;
    private bool isInteraction;

    private bool isInitialized;

    [Inject]
    public void Construct(Money money, GoldenChips goldenChips, GameStateManager stateManager)
    {
        this.stateManager = stateManager;
    }

    private void Init()
    {
        isInitialized = true;
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
        if (isInitialized == false) Init();

    }

    public void FinishInteraction()
    {
        camera.gameObject.SetActive(false);
        isInteraction = false;
        shop.FinishInteraction();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.PlaqueShopping)
        {
            StartInteraction();
        }
        else if (isInteraction == true)
        {
            FinishInteraction();
        }
    }
}
