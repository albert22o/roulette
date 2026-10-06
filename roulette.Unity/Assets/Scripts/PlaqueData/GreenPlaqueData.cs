using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreenPlaqueData", menuName = "Plaques/GreenPlaqueData")]
public class GreenPlaqueData : PlaqueData
{
    public override bool CanUse(IReadOnlyDictionary<int, BetInfo> pocketMapping)
    {
        return true;
    }

    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new GreenPlaque(this, dependencies);
    }
}
