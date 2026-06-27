using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using static GameStateManager;

public class PlaqueShopFieldSelectionInteractionHandler : MonoBehaviour
{
    [SerializeField] private CinemachineCamera tableCamera;
    [SerializeField] private TableSelection tableSelection;
    [SerializeField] private Canvas canvas;

    private GameStateManager stateManager;
    private bool isInteraction;

    [Inject]
    public void Construct(GameStateManager stateManager)
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
        }
        else if (isInteraction == true)
        {
            FinishInteraction();
        }
    }
}
