public abstract class Chip
{
    public ChipData ChipData { get; }

    public Chip(ChipData chipData)
    {
        ChipData = chipData;
    }
    public abstract void Apply();
    public abstract void Revert();
}
