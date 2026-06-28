using UnityEngine;

[CreateAssetMenu(fileName = "GreenPlaqueData", menuName = "Plaques/GreenPlaqueData")]
public class GreenPlaqueData : PlaqueData
{
    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new GreenPlaque(this, dependencies);
    }
}
