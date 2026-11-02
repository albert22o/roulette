using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlaqueShopFieldSelectionInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera tableCamera;
    [SerializeField] private TableSelectionView tableSelection;
    [SerializeField] private GameObject fields;


    private GameStateManager stateManager;
    private bool isInteraction;

    [Inject]
    public void Construct(GameStateManager stateManager)
    {
        this.stateManager = stateManager;
        stateManager.GameStateChanged += OnGameStateChanged;
    }


    private void Start()
    {
        if (stateManager.CurrentState is not GameStateManager.GameState.FieldsSelecting)
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        stateManager.GameStateChanged -= OnGameStateChanged;
    }

    public void StartInteraction()
    {
        tableCamera.gameObject.SetActive(true);
        fields.gameObject.SetActive(true);
        isInteraction = true;
        tableSelection.StartInteraction();
    }

    public void FinishInteraction()
    {
        tableCamera.gameObject.SetActive(false);
        fields.gameObject.SetActive(false);
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
