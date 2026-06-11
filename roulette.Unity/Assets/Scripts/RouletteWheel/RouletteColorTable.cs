using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RouletteColorTable", menuName = "Scriptable Objects/RouletteColorTable")]
public class RouletteColorTable : ScriptableObject
{
    [SerializeField] private List<ColorsTable> colorsList;

    private Dictionary<ColorType, Color> colors;

    private Dictionary<ColorType, Color> Colors
    {
        get
        {
            if (colors == null)
            {
                colors = new Dictionary<ColorType, Color>();
                foreach (var c in colorsList)
                    colors[c.ColorName] = c.Color;
            }
            return colors;
        }
    }

    public Color GetColorByName(ColorType name) => Colors[name];
}

[System.Serializable]
internal struct ColorsTable
{
    public ColorType ColorName;
    public Color Color;
}