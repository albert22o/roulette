using UnityEngine;

public abstract class ChipData : ItemData
{
    [SerializeField] private int cost;
        
    public GameObject Prefab;
    public int Cost => cost;
    public abstract Chip CreateChip(ChipDependencies deps);
}
