using System;
using System.Collections.Generic;

public class PlaqueFieldsSelector
{
    private List<int> currentSelectedFields = new();
    private PlaqueData plaqueData;
    private int selectionIndex;
    private List<int[]> selectedAll = new();
    private SelectionRequest currentSelectionRequest;
    private TableConfig tableConfig;

    public event Action<int[][]> SelectionCompleted;
    public event Action<SelectionRequest> SelectionRequestChanged;
    public event Action<bool> IsSelectionValidChanged;



    public PlaqueFieldsSelector(TableConfig tableConfig)
    {
        this.tableConfig = tableConfig;
    }

    public void SetPlaque(PlaqueData plaqueData)
    {
        this.plaqueData = plaqueData;
        currentSelectedFields.Clear();
        selectedAll.Clear();
        selectionIndex = 0;
        currentSelectionRequest = plaqueData.SelectionRequests[0];
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
        selectedAll.Add(currentSelectedFields.ToArray());
        selectionIndex++;
        if (selectionIndex >= plaqueData.SelectionRequests.Length)
        {
            SelectionCompleted?.Invoke(selectedAll.ToArray());
        }
        else
        {
            currentSelectionRequest = plaqueData.SelectionRequests[selectionIndex];
            SelectionRequestChanged?.Invoke(currentSelectionRequest);
            IsSelectionValidChanged?.Invoke(false);
        }
    }
}
