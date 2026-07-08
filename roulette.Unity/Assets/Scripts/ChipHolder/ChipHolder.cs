using System;
using UnityEngine;
using System.Collections.Generic;

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
    public int FreeSlotCount
    {
        get
        {
            var freeSlotIndex = FindFreeSlot();
            if (freeSlotIndex == -1)
                return 0;
            return Capacity - freeSlotIndex;
        }
    }
    public IReadOnlyList<Chip> Chips => chips;

    public ChipHolder(RoundStateManager roundStateManager, ChipFactory chipFactory, ChipHolderConfig config)
    {
        this.chipFactory = chipFactory;
        Capacity = config.Capacity;
        chips = new Chip[Capacity];

        foreach (var chip in config.StartChips) 
            AddChip(chip);

        roundStateManager.StateChanged += OnStateChanged; 
    }

    public void AddChip(ChipData chipData)
    {
        int freeIndex = FindFreeSlot();
        if (freeIndex == -1)
            return;
        
        chips[freeIndex] = chipFactory.Create(chipData, this);
        chips[freeIndex].OnAddedToHolder();

        ChipAdded?.Invoke(freeIndex, chips[freeIndex]);
    }

    public void RemoveChip(int index)
    {
        if (!IsValidIndex(index) || chips[index] == null)
            return;

        chips[index].OnRemovedFromHolder();
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
                chips[i].OnRemovedFromHolder();
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