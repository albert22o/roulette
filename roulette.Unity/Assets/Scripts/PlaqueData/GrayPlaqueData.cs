using UnityEngine;

[CreateAssetMenu(fileName = "GrayPlaqueData", menuName = "Plaques/GrayPlaqueData")]
public class GrayPlaqueData : PlaqueData
{
    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new VioletPlaque(this, dependencies);
    }
}
