
using System;

public class PlaqueUseController
{
    private GameStateManager stateManager;
    private PlaqueFieldsSelector fieldsSelector;
    private PlaqueHolder plaqueHolder;
    private Plaque plaque;
    private PlaqueFactory plaqueFactory;
    public event Action PlaqueUsed;

    public PlaqueUseController(GameStateManager stateManager,
        PlaqueFieldsSelector fieldsSelector, PlaqueHolder plaqueHolder, PlaqueFactory plaqueFactory)
    {
        this.stateManager = stateManager;
        this.fieldsSelector = fieldsSelector;
        this.plaqueHolder = plaqueHolder;
        this.plaqueFactory = plaqueFactory;

        fieldsSelector.SelectionCompleted += OnSelectionCompleted;
    }

    public void UsePlaque(PlaqueData data)
    {
        plaque = plaqueFactory.CreatePlaque(data);
        if (plaque.SelectionRequests == null || plaque.SelectionRequests.Length == 0)
        {
            plaqueHolder.AddPlaque(plaque, null);
            PlaqueUsed?.Invoke();
            plaque = null;
        }
        else if (plaque.SelectionRequests.Length > 0)
        {
            fieldsSelector.SetPlaque(plaque);
            stateManager.ChangeGameState(GameStateManager.GameState.FieldsSelecting);
        }
    }

    public void OnSelectionCompleted(int[][] selectedFields)
    {
        plaqueHolder.AddPlaque(plaque, selectedFields);
        stateManager.ChangeGameState(GameStateManager.GameState.PlaqueShopping);
        PlaqueUsed?.Invoke();
        plaque = null;
    }
}
