using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ColorChangeSelectedPlaque", menuName = "Plaques/ColorChangeSelectedPlaque")]
public class ColorChangeSelectedPlaqueData : PlaqueData
{
    [field: SerializeField] public int SelectionCount { get; private set; }
    [field: SerializeField] public ColorMask Filter { get; private set; }
    [field: SerializeField] public ColorType TargetColor { get; private set; }
    [SerializeField] private LocalizedString hint;
    public string Hint => hint.Resolve();

    public override Plaque CreatePlaque(PlaqueDependencies dependencies)
    {
        return new ColorChangeSelectedPlaque(this, dependencies);
    }

    public override bool CanUse(IReadOnlyDictionary<int, BetInfo> pocketMapping)
    {
        foreach (var field in pocketMapping)
        {
            var color = field.Value.FieldColor;
            if ((Filter & color.ToMask()) != 0 && color != TargetColor)
                return true;
        }
        return false;
    }
}
