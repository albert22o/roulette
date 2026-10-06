using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrayPlaqueData", menuName = "Plaques/GrayPlaqueData")]
public class GrayPlaqueData : PlaqueData
{
    public override bool CanUse(IReadOnlyDictionary<int, BetInfo> pocketMapping)
    {
        for (int i = 0; i < pocketMapping.Count; i++)
        {
            if (pocketMapping[i].FieldColor != ColorType.Violet) return true;
        }
        return false;
    }

    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new VioletPlaque(this, dependencies);
    }

}
