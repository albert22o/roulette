using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class PlaqueShopInteractionHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private new CinemachineCamera camera;
    [SerializeField] private PlaqueShopView shop;
    [SerializeField] private FPCInteractionPositionSetter positionSetter;

    private GameStateManager stateManager;
    private bool isInteraction;

    private bool isInitialized;

    public string Tooltip => Loc.Get("interact.plaque_machine");

    public bool CanInteract => true;

    [Inject]
    public void Construct(GameStateManager stateManager)
    {
        this.stateManager = stateManager;
        stateManager.GameStateChanged += OnGameStateChanged;
    }

    private void Init()
    {
        isInitialized = true;
    }

    public void StartInteraction()
    {
        stateManager.ChangeGameState(GameState.PlaqueShopping);
        camera.gameObject.SetActive(true);
        isInteraction = true;
        shop.StartInteraction();
        if (isInitialized == false) Init();
    }

    public void FinishInteraction()
    {
        positionSetter.ChangePlayerPositionAndRotation();
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

    public void Interact()
    {
        StartInteraction();
    }
}
