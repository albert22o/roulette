using System.Collections.Generic;

public abstract class Chip
{
    public ChipData ChipData { get; }
    protected Chip(ChipData chipData)
    {
        ChipData = chipData;
    }
    public abstract void Apply();
    public abstract void Revert();

    public virtual void OnAddedToHolder() { }
    public virtual void OnRemovedFromHolder() { }
}
