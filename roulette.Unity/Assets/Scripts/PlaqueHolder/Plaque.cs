using UnityEngine;

public abstract class Plaque
{
    public PlaqueData PlaqueData { get; }
    public Plaque(PlaqueData data)
    {
        PlaqueData = data;
    }
    public abstract void Apply(PlaqueDependencies dependencies);

}
