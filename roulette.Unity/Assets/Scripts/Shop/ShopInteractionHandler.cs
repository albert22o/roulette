using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class ShopInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera shopCamera;
    [SerializeField] private Shop shop;
    [SerializeField] private FirstBallLoader firstBallLoader;

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
        firstBallLoader.LoadBall();
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
        shopCamera.gameObject.SetActive(true);
        isInteraction = true;
        shop.StartInteraction();
        if (isInitialized == false) Init();

    }

    public void FinishInteraction()
    {
        shopCamera.gameObject.SetActive(false);
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
