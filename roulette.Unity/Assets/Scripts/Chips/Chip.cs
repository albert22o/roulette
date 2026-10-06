using System.Collections.Generic;

public abstract class Chip
{
    public ChipData ChipData { get; }
    public abstract OperationType Operation { get; }
    protected Chip(ChipData chipData)
    {
        ChipData = chipData;
    }
    public abstract float AppliedValue { get; }

    public abstract bool Apply();
    public abstract void Revert();

    public virtual void OnAddedToHolder() { }
    public virtual void OnRemovedFromHolder() { }
}

public enum OperationType
{
    None,
    Multiply,
    Add
}
