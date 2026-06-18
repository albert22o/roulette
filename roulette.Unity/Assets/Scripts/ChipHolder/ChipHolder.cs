using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ChipHolder
{
    public event Action<int, Chip> ChipAdded;       
    public event Action<int> ChipRemoved;           
    public event Action<int, int> ChipsSwapped;
    public event Action<int> ChipAnimated;
    public int Capacity { get; }

    private Chip[] chips;
    private bool isActive = false;
    private readonly ChipFactory chipFactory;
    public bool HasFreeSlot => FindFreeSlot() != -1;
    public IReadOnlyList<Chip> Chips => chips;

    public ChipHolder(RoundStateManager roundStateManager, ChipFactory chipFactory, int capacity = 5)
    {
        chips = new Chip[capacity];
        roundStateManager.StateChanged += OnStateChanged;
        Capacity = capacity; 
        this.chipFactory = chipFactory;
    }

    public void AddChip(ChipData chipData)
    {
        int freeIndex = FindFreeSlot();
        if (freeIndex == -1)
            return;

        chips[freeIndex] = chipFactory.Create(chipData);

        ChipAdded?.Invoke(freeIndex, chips[freeIndex]);
    }

    public void RemoveChip(int index)
    {
        if (!IsValidIndex(index) || chips[index] == null)
            return;

        chips[index].Revert();
        chips[index] = null;

        ChipRemoved?.Invoke(index);
    }

    public void RemoveChip(ChipData data)
    {
        if (data == null)
            return;

        for (int i = 0; i < chips.Length; i++)
        {
            if (chips[i] != null && chips[i].ChipData == data)
            {
                chips[i].Revert();
                chips[i] = null;
                ChipRemoved?.Invoke(i);
                return;
            }
        }
    }

    public void SwapChips(int indexA, int indexB)
    {
        if (!isActive || !IsValidIndex(indexA) || !IsValidIndex(indexB))
            return;

        if (indexA == indexB)
            return;

        (chips[indexA], chips[indexB]) = (chips[indexB], chips[indexA]);

        ChipsSwapped?.Invoke(indexA, indexB);
    }

    public void RevertAll()
    {
        for (int i = chips.Length - 1; i >= 0; i--)
            chips[i]?.Revert();
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        isActive = newState == RoundStateManager.RoundState.PlacingBets;
    }

    private void ApplyAll()
    {
        for (int i = 0; i < chips.Length; i++)
            chips[i]?.Apply();
    }

    public void ApplyByIndex(int index)
    {
        chips[index]?.Apply();
        ChipAnimated?.Invoke(index);
        
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < chips.Length; i++)
            if (chips[i] == null) return i;
        return -1;
    }

    private bool IsValidIndex(int index) =>
        index >= 0 && index < chips.Length;
}