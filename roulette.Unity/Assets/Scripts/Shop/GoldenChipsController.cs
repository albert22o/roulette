using System;
using UnityEngine;

public class GoldenChipsController : MonoBehaviour
{
    public int Count { get; private set; }

    public event Action<int> CountChanged;


    public void SetChips(int count)
    {
        Count = count;
        CountChanged?.Invoke(Count);
    }

    public void AddChips(int count)
    {
        Count += count;
        CountChanged?.Invoke(Count);
    }

    public void RemoveChips(int count)
    {
        Count = Mathf.Max(0, Count - count);
        CountChanged?.Invoke(Count);
    }
}
