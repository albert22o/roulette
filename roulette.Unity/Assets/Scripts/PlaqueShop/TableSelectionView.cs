using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class TableSelectionView : MonoBehaviour
{
    [SerializeField] List<BetButton> betButtons = new();

    private TableConfig tableConfig;
    private PlaqueFieldsSelector plaqueFieldsSelector;


    [Inject]
    public void Construct(TableConfig tableConfig,
        RoundStateManager roundStateManager, PlaqueFieldsSelector fieldsSelector)
    {
        this.tableConfig = tableConfig;
        this.plaqueFieldsSelector = fieldsSelector;
    }

    public void Start()
    {
        if (betButtons.Count < tableConfig.PocketMapping.Count)
        {
            Debug.LogError("Не все кнопки для ставок проставлены");
            return;
        }

        tableConfig.PocketNumberChanged += OnPocketNumberChanged;
        tableConfig.PocketColorChanged += OnPocketColorChanged;
        plaqueFieldsSelector.SelectionPhaseCompleted += ClearHighlight;
        

        foreach (var pocket in tableConfig.PocketMapping)
        {
            betButtons[pocket.Key].Init(pocket.Value,
                () => OnSelectField(pocket.Key),
                () => OnSelectField(pocket.Key));
        }
    }

    public void OnSelectField(int buttonIndex)
    {
        if (plaqueFieldsSelector.ToggleField(buttonIndex))
        {
            betButtons[buttonIndex].Highlight(true, BetButton.HighlightPhase.Number);
        }
        else
        {
            betButtons[buttonIndex].Highlight(false, BetButton.HighlightPhase.Number);
        }
    }

    private void OnDestroy()
    {
        tableConfig.PocketNumberChanged -= OnPocketNumberChanged;
        tableConfig.PocketColorChanged -= OnPocketColorChanged;
        plaqueFieldsSelector.SelectionPhaseCompleted -= ClearHighlight;
    }

    private void OnPocketColorChanged(int index, ColorType color)
    {
        betButtons[index].Color = color;
    }

    private void OnPocketNumberChanged(int index, int fieldNum)
    {
        betButtons[index].FieldNum = fieldNum;
    }

    public void StartInteraction()
    {

    }

    public void ClearHighlight()
    {
        foreach (var betButton in betButtons) 
        {
            betButton.Highlight(false, BetButton.HighlightPhase.Number);
        }
    }
}
