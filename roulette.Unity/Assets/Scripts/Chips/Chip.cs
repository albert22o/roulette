using System.Collections.Generic;

public abstract class Chip
{
    public ChipData ChipData { get; }
    protected HashSet<string> EligibleBallHashes { get; private set; } = new HashSet<string>();
    public Chip(ChipData chipData)
    {
        ChipData = chipData;
    }
    public abstract void Apply();
    public abstract void Revert();

    protected void AddToMemory(string ballHash)
    {
        EligibleBallHashes.Add(ballHash);
    }

    protected bool IsInMemory(string ballHash)
    {
        return EligibleBallHashes.Contains(ballHash);
    }

    protected void ClearMemory()
    {
        EligibleBallHashes.Clear();
    }
}
