using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TableSelectionView : MonoBehaviour
{
    [SerializeField] List<FieldPlaqueSelectionPhase> betButtons = new();
    [SerializeField] private float delayToChangeState = 2f;
    [SerializeField] private PlaqueShopFieldSelectionInteractionHandler interactionHandler;

    private TableConfig tableConfig;
    private PlaqueFieldsSelector plaqueFieldsSelector;
    private PlaqueUseController plaqueUseController;
    private GameStateManager gameStateManager;


    [Inject]
    public void Construct(TableConfig tableConfig,
        RoundStateManager roundStateManager, PlaqueFieldsSelector fieldsSelector, PlaqueUseController plaqueUseController,
        GameStateManager gameStateManager)
    {
        this.tableConfig = tableConfig;
        this.plaqueFieldsSelector = fieldsSelector;
        this.plaqueUseController = plaqueUseController;
        this.gameStateManager = gameStateManager;
    }

    public void Start()
    {
        if (betButtons.Count < tableConfig.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        plaqueFieldsSelector.SelectionPhaseCompleted += ClearHighlight;
        plaqueUseController.PlaqueUsed += OnPlaqueUsed;

        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => OnSelectField(pocket.Key));
        }
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
        for (int i = 0; i < tableConfig.PocketMapping.Count; i++)
        {
            betButtons[i].UpdateField(false);
        }
    }

    public void StartInteraction()
    {
        for (int i = 0; i < tableConfig.PocketMapping.Count; i++)
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
        for (int i = 0; i < tableConfig.PocketMapping.Count; i++)
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
}
