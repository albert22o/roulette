using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TableSelectionView : MonoBehaviour
{
    [SerializeField] List<FieldPlaqueSelectionPhase> betButtons = new();
    [SerializeField] private float delayToChangeState = 2f;
    [SerializeField] private PlaqueShopFieldSelectionInteractionHandler interactionHandler;

    private TableInfo tableInfo;
    private PlaqueFieldsSelector plaqueFieldsSelector;
    private PlaqueUseController plaqueUseController;
    private GameStateManager gameStateManager;
    private CameraEventHandler cameraEventHandler;

    private bool isCameraArrived;
    private Action CameraArrived;


    [Inject]
    public void Construct(TableInfo tableInfo,
        RoundStateManager roundStateManager, PlaqueFieldsSelector fieldsSelector, PlaqueUseController plaqueUseController,
        GameStateManager gameStateManager, CameraEventHandler cameraEventHandler)
    {
        this.tableInfo = tableInfo;
        this.plaqueFieldsSelector = fieldsSelector;
        this.plaqueUseController = plaqueUseController;
        this.gameStateManager = gameStateManager;
        this.cameraEventHandler = cameraEventHandler;
    }

    public void Start()
    {
        if (betButtons.Count < tableInfo.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        plaqueFieldsSelector.SelectionPhaseCompleted += ClearHighlight;
        plaqueUseController.PlaqueUsed += OnPlaqueUsed;

        foreach (var pocket in tableInfo.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => OnSelectField(pocket.Key));
        }
        cameraEventHandler.TransitionFinished += OnCameraTransitionFinished;
    }

    public void OnSelectField(int buttonIndex)
    {
        if (plaqueFieldsSelector.ToggleField(buttonIndex))
        {
            betButtons[buttonIndex].Highlight(true);
        }
        else
        {
            betButtons[buttonIndex].Highlight(false);
        }
    }

    public void PlayFieldsChangeAnimation()
    {
        for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
        {
            betButtons[i].UpdateField(false);
        }
    }

    public void StartInteraction()
    {
        CameraArrived = null;
        isCameraArrived = false;
        for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
        {
            betButtons[i].UpdateField(true);
        }
    }

    public void ClearHighlight()
    {
        foreach (var betButton in betButtons)
        {
            betButton.Highlight(false);
        }
    }

    private void OnPlaqueUsed()
    {
        if (isCameraArrived == false)
        {
            CameraArrived = OnPlaqueUsed;
            return;
        }
        for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
        {
            betButtons[i].UpdateField(false);
        }
        StartCoroutine(WaitBeforeExit());
    }

    private IEnumerator WaitBeforeExit()
    {
        yield return new WaitForSeconds(delayToChangeState);
        interactionHandler.FinishInteraction();
        gameStateManager.ChangeGameState(GameStateManager.GameState.PlaqueShopping);
    }

    private void OnCameraTransitionFinished()
    {
        isCameraArrived = true;
        CameraArrived?.Invoke();
        CameraArrived = null;
    }
}
