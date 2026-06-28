using System;
using System.Collections.Generic;

public class PlaqueFieldsSelector
{
    private List<int> currentSelectedFields = new();
    private Plaque plaque;
    private int selectionIndex;
    private List<int[]> selectedAll = new();
    private SelectionRequest currentSelectionRequest;
    private TableConfig tableConfig;

    public event Action<int[][]> SelectionCompleted;
    public event Action<SelectionRequest> SelectionRequestChanged;
    public event Action<bool> IsSelectionValidChanged;
    public event Action SelectionPhaseCompleted;

    public SelectionRequest CurrentRequest => currentSelectionRequest;



    public PlaqueFieldsSelector(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public void SetPlaque(Plaque plaque)
    {
        this.plaque = plaque;
        currentSelectedFields.Clear();
        selectedAll.Clear();
        selectionIndex = 0;
        currentSelectionRequest = plaque.SelectionRequests[0];
        SelectionRequestChanged?.Invoke(currentSelectionRequest);
        IsSelectionValidChanged?.Invoke(false);
    }

    public bool ToggleField(int index)
    {
        if (currentSelectedFields.Contains(index) == true)
        {
            currentSelectedFields.Remove(index);
            IsSelectionValidChanged?.Invoke(false);
            return false;
        }
        else
        {
            if (currentSelectionRequest.Filter != null && currentSelectionRequest.Filter(tableConfig.PocketMapping[index]) == false)
            {
                return false;
            }
            if (currentSelectedFields.Count >= currentSelectionRequest.Count)
            {
                return false;
            }
            currentSelectedFields.Add(index);
            if (currentSelectedFields.Count == currentSelectionRequest.Count)
            {
                IsSelectionValidChanged?.Invoke(true);
            }
            return true;
        }
    }
    public void ApplySelection()
    {
        var currentSelectedFielsArray = currentSelectedFields.ToArray();
        selectedAll.Add(currentSelectedFielsArray);
        plaque.OnSelectionPhaseCompleted(selectionIndex, currentSelectedFielsArray);
        selectionIndex++;
        if (selectionIndex >= plaque.SelectionRequests.Length)
        {
            SelectionCompleted?.Invoke(selectedAll.ToArray());
            plaque = null;
        }
        else
        {
            currentSelectionRequest = plaque.SelectionRequests[selectionIndex];
            SelectionPhaseCompleted?.Invoke();
            SelectionRequestChanged?.Invoke(currentSelectionRequest);
            IsSelectionValidChanged?.Invoke(false);
            currentSelectedFields.Clear();
        }
    }
}
