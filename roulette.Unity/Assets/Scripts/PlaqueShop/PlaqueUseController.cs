
using System;

public class PlaqueUseController
{
    private GameStateManager stateManager;
    private PlaqueFieldsSelector fieldsSelector;
    private PlaqueHolder plaqueHolder;
    private PlaqueData selectedPlaque;

    public event Action PlaqueUsed;

    public PlaqueUseController(GameStateManager stateManager,
        PlaqueFieldsSelector fieldsSelector, PlaqueHolder plaqueHolder)
    {
        this.stateManager = stateManager;
        this.fieldsSelector = fieldsSelector;
        this.plaqueHolder = plaqueHolder;

        fieldsSelector.SelectionCompleted += OnSelectionCompleted;
    }

    public void UsePlaque(PlaqueData data)
    {
        selectedPlaque = data;
        if (data.SelectionRequests.Length > 0)
        {
            fieldsSelector.SetPlaque(selectedPlaque);
            stateManager.ChangeGameState(GameStateManager.GameState.FieldsSelecting);
        }
        else
        {
            plaqueHolder.AddPlaque(selectedPlaque, null);
        }
    }

    public void OnSelectionCompleted(int[][] selectedFields)
    {
        plaqueHolder.AddPlaque(selectedPlaque, selectedFields);
        stateManager.ChangeGameState(GameStateManager.GameState.PlaqueShopping);
        PlaqueUsed?.Invoke();
    }
}
