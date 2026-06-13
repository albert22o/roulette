using System;

public class ChipHolder
{
    public event Action<int, Chip> ChipAdded;       
    public event Action<int> ChipRemoved;           
    public event Action<int, int> ChipsSwapped;
    public event Action<int> ChipAnimated;
    public int Capacity { get; }

    public Chip[] Сhips { get; private set; }
    private bool isActive = false;
    private readonly ChipFactory chipFactory;

    public ChipHolder(RoundStateManager roundStateManager, ChipFactory chipFactory, int capacity = 5)
    {
        Сhips = new Chip[capacity];
        roundStateManager.StateChanged += OnStateChanged;
        Capacity = capacity; 
        this.chipFactory = chipFactory;
    }

    public void AddChip(ChipData chipData)
    {
        int freeIndex = FindFreeSlot();
        if (freeIndex == -1)
            return;

        Сhips[freeIndex] = chipFactory.Create(chipData);

        ChipAdded?.Invoke(freeIndex, Сhips[freeIndex]);
    }

    public void RemoveChip(int index)
    {
        if (!IsValidIndex(index) || Сhips[index] == null)
            return;

        Сhips[index].Revert();
        Сhips[index] = null;

        ChipRemoved?.Invoke(index);
    }

    public void RemoveChip(ChipData data)
    {
        if (data == null)
            return;

        for (int i = 0; i < Сhips.Length; i++)
        {
            if (Сhips[i] != null && Сhips[i].ChipData == data)
            {
                Сhips[i].Revert();
                Сhips[i] = null;
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

        (Сhips[indexA], Сhips[indexB]) = (Сhips[indexB], Сhips[indexA]);

        ChipsSwapped?.Invoke(indexA, indexB);
    }

    public void RevertAll()
    {
        for (int i = Сhips.Length - 1; i >= 0; i--)
            Сhips[i]?.Revert();
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        isActive = newState == RoundStateManager.RoundState.PlacingBets;
    }

    private void ApplyAll()
    {
        for (int i = 0; i < Сhips.Length; i++)
            Сhips[i]?.Apply();
    }

    public void ApplyByIndex(int index)
    {
        Сhips[index]?.Apply();
        ChipAnimated?.Invoke(index);
        
    }

    private int FindFreeSlot()
    {
        for (int i = 0; i < Сhips.Length; i++)
            if (Сhips[i] == null) return i;
        return -1;
    }

    private bool IsValidIndex(int index) =>
        index >= 0 && index < Сhips.Length;
}