using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class PlaqueShopFieldSelectionInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera tableCamera;
    [SerializeField] private TableSelectionView tableSelection;
    [SerializeField] private Canvas canvas;

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
        if (stateManager.CurrentState is not GameState.FieldsSelecting) gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        stateManager.GameStateChanged -= OnGameStateChanged;
    }

    public void StartInteraction()
    {
        tableCamera.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        isInteraction = true;
        tableSelection.StartInteraction();
    }

    public void FinishInteraction()
    {
        tableCamera.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        isInteraction = false;
        tableSelection.FinishInteraction();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.FieldsSelecting)
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
