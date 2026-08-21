using System;
using System.Collections.Generic;

public class ChipHolder
{
    public event Action<int, Chip> ChipAdded;
    public event Action<int> ChipRemoved;
    public event Action<int, int> ChipsSwapped;
    public event Action<int, int> ChipMoved;
    public event Action<int> ChipAnimated;

    public event Action<int> SlotAdded;
    public event Action<int> SlotRemoved;

    public int Capacity { get; private set; }
    private Chip[] chips;
    private bool isActive = false;
    private readonly ChipFactory chipFactory;

    public bool HasFreeSlot => FindFreeSlot() != -1;

    public int FreeSlotCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < chips.Length; i++)
                if (chips[i] == null)
                    count++;
            return count;
        }
    }
    public IReadOnlyList<Chip> Chips => chips;

    public ChipHolder(RoundStateManager roundStateManager, BetCalculator betCalculator, ChipFactory chipFactory, ChipHolderConfig config)
    {
        this.chipFactory = chipFactory;
        Capacity = config.Capacity;
        chips = new Chip[Capacity];
        foreach (var chip in config.StartChips)
            AddChip(chip);
        roundStateManager.StateChanged += OnStateChanged;
        betCalculator.GetReward += (_) => RevertAll();
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

    /// <summary>
    /// Добавляет один новый пустой слот в конец holder'а.
    /// </summary>
    public void AddSlot()
    {
        var newChips = new Chip[Capacity + 1];
        Array.Copy(chips, newChips, chips.Length);
        chips = newChips;
        Capacity++;

        SlotAdded?.Invoke(Capacity - 1);
    }

    /// <summary>
    /// Добавляет несколько слотов сразу.
    /// </summary>
    public void AddSlots(int count)
    {
        for (int i = 0; i < count; i++)
            AddSlot();
    }

    public void RemoveSlot(int index)
    {
        if (!IsValidIndex(index))
            return;

        // Сначала снимаем фишку, если есть
        if (chips[index] != null)
            RemoveChip(index);

        int lastIndex = Capacity - 1;

        for (int i = index + 1; i <= lastIndex; i++)
        {
            chips[i - 1] = chips[i];
            if (chips[i - 1] != null)
                ChipMoved?.Invoke(i, i - 1);
        }
        chips[lastIndex] = null;

        var newChips = new Chip[Capacity - 1];
        Array.Copy(chips, newChips, newChips.Length);
        chips = newChips;
        Capacity--;

        SlotRemoved?.Invoke(lastIndex);
    }

    /// <summary>
    /// Удаляет последний слот holder'а.
    /// </summary>
    public void RemoveLastSlot()
    {
        if (Capacity <= 0)
            return;
        RemoveSlot(Capacity - 1);
    }

    public void RemoveLastSlots(int count)
    {
        for (int i = 0; i < count; i++)
            RemoveLastSlot();
    }

    private void OnStateChanged(RoundStateManager.RoundState newState)
    {
        isActive = newState == RoundStateManager.RoundState.PlacingBets;
    }

    public void ApplyByIndex(int index)
    {
        if (chips[index]?.Apply() == true)
        {
            ChipAnimated?.Invoke(index);
        }
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