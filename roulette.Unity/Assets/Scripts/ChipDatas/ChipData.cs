using UnityEngine;

public abstract class ChipData : ScriptableObject
{
    public GameObject Prefab;
    public abstract Chip CreateChip(ChipDependencies deps);
}
