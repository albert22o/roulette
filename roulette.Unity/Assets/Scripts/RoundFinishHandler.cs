using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class RoundFinishHandler : MonoBehaviour
{
    [SerializeField] private FPCInteractionPositionSetter playerPositionSetter;

    private GameStateManager stateManager;
    private bool isInteraction;

    [Inject]
    public void Construct(GameStateManager stateManager)
    {
        this.stateManager = stateManager;
    }


    private void Start()
    {
        stateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        stateManager.GameStateChanged -= OnGameStateChanged;
    }

    public void StartInteraction()
    {
        isInteraction = true;
    }

    public void FinishInteraction()
    {
        isInteraction = false;
        playerPositionSetter.ChangePlayerPositionAndRotation();
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.BettingСycle)
        {
            StartInteraction();
            gameObject.SetActive(true);
        }
        else if (isInteraction == true)
        {
            FinishInteraction();
            gameObject.SetActive(false);
        }
    }
}
