using UnityEngine;

public abstract class PlaqueData : ItemData
{
    public abstract SelectionRequest[] SelectionRequests { get; }

    public abstract Plaque CreatePlaque();
}
