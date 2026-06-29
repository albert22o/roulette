using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorChangeSelectedPlaque", menuName = "Plaques/ColorChangeSelectedPlaque")]
public class ColorChangeSelectedPlaqueData : PlaqueData
{
    [field: SerializeField] public int SelectionCount { get; private set; }
    [field: SerializeField] public ColorMask filter { get; private set; }
    [field: SerializeField] public ColorType TargetColor { get; private set; }
    [TextArea(1, 5)]
    [SerializeField] public string hint;
    public string Hint => hint;

    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new ColorChangeSelectedPlaque(this, dependencies);
    }
}
