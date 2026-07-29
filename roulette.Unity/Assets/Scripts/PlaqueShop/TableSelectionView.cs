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
                pocket.Key);
            betButtons[pocket.Key].HoverEnter += OnFieldHoverEnter;
            betButtons[pocket.Key].HoverExit += OnFieldHoverExit;
            betButtons[pocket.Key].Clicked += OnFieldClick;
        }
        cameraEventHandler.TransitionFinished += OnCameraTransitionFinished;
    }

    private void OnDestroy()
    {
        foreach (var pocket in tableInfo.PocketMapping)
        {
            betButtons[pocket.Key].HoverEnter -= OnFieldHoverEnter;
            betButtons[pocket.Key].HoverExit -= OnFieldHoverExit;
            betButtons[pocket.Key].Clicked -= OnFieldClick;
        }
        cameraEventHandler.TransitionFinished -= OnCameraTransitionFinished;
        plaqueFieldsSelector.SelectionPhaseCompleted -= ClearHighlight;
        plaqueUseController.PlaqueUsed -= OnPlaqueUsed;
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

    private void OnFieldHoverEnter(FieldPlaqueSelectionPhase field)
    {
        if (plaqueFieldsSelector.CanToggle(field.Index))
        {
            field.ToggleOutline(true);
        }
    }

    private void OnFieldHoverExit(FieldPlaqueSelectionPhase field)
    {
            field.ToggleOutline(false);
    }

    private void OnFieldClick(FieldPlaqueSelectionPhase field)
    {
        if (plaqueFieldsSelector.ToggleField(field.Index))
        {
            field.Highlight(true);
        }
        else
        {
            field.Highlight(false);
        }
    }
}
