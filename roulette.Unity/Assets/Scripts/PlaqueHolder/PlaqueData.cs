using System.Collections.Generic;

public abstract class PlaqueData : ItemData
{
    public abstract Plaque CreatePlaque(PlaqueDependencies dependencies);
    public abstract bool CanUse(IReadOnlyDictionary<int, BetInfo> pocketMapping);
}
