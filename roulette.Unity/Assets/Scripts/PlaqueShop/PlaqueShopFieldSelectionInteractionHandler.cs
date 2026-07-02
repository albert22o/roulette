using Unity.Cinemachine;
using UnityEngine;
using Zenject;

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
        if (stateManager.CurrentState is not GameStateManager.GameState.FieldsSelecting) gameObject.SetActive(false);
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
        tableSelection.ClearHighlight();
    }

    private void OnGameStateChanged(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.FieldsSelecting)
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
