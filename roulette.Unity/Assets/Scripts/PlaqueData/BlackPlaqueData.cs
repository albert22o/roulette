using UnityEngine;

[CreateAssetMenu(fileName = "BlackPlaqueData", menuName = "Plaques/BlackPlaqueData")]
public class BlackPlaqueData : PlaqueData
{
    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new BlackPlaque(this, dependencies);
    }
}
