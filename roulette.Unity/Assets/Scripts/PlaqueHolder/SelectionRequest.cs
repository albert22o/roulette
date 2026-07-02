using System;
[Serializable]
public class SelectionRequest
{
    public int Count;             
    public Func<BetInfo, bool> Filter;  
    public string Hint;             
}