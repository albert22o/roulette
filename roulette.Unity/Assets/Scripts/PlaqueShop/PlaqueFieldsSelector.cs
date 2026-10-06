using System;
using System.Collections.Generic;

public class PlaqueFieldsSelector
{
    private List<int> currentSelectedFields = new();
    private Plaque plaque;
    private int selectionIndex;
    private List<int[]> selectedAll = new();
    private SelectionRequest currentSelectionRequest;
    private readonly TableInfo tableInfo;

    public event Action<int[][]> SelectionCompleted;
    public event Action<SelectionRequest> SelectionRequestChanged;
    public event Action<bool> IsSelectionValidChanged;
    public event Action SelectionPhaseCompleted;

    public SelectionRequest CurrentRequest => currentSelectionRequest;

    public PlaqueFieldsSelector(TableInfo tableInfo)
    {
        this.tableInfo = tableInfo;
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

    public bool CanToggle(int index)
    {
        if (currentSelectionRequest == null)
            return false;
        if (currentSelectedFields.Contains(index))
            return true;
        if (currentSelectionRequest.Filter != null && !currentSelectionRequest.Filter(tableInfo.PocketMapping[index]))
            return false;
        if (currentSelectedFields.Count >= currentSelectionRequest.Count)
            return false;
        return true;
    }

    public bool CanUse(int index)
    {
        if (currentSelectionRequest.Filter != null && !currentSelectionRequest.Filter(tableInfo.PocketMapping[index]))
            return false;
        return true;
    }

    public bool ToggleField(int index)
    {
        if (currentSelectionRequest == null)
        {
            return false;
        }
        if (currentSelectedFields.Contains(index) == true)
        {
            currentSelectedFields.Remove(index);
            IsSelectionValidChanged?.Invoke(false);
            return false;
        }
        else
        {
            if (currentSelectionRequest.Filter != null && currentSelectionRequest.Filter(tableInfo.PocketMapping[index]) == false)
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
            else
            {
                int availableToUseFields = 0;
                for (int i = 0; i < tableInfo.PocketMapping.Count; i++)
                {
                    if (currentSelectedFields.Contains(i))
                        continue;
                    if (CanUse(i))
                        availableToUseFields++;
                }
                if (availableToUseFields == 0)
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
            SelectionPhaseCompleted?.Invoke();
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
