using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RouletteColorTable", menuName = "Scriptable Objects/RouletteColorTable")]
public class RouletteColorTable : ScriptableObject
{
    [SerializeField] private List<ColorsTable> colorsList;

    private Dictionary<string, Color> colors;

    private Dictionary<string, Color> Colors
    {
        get
        {
            if (colors == null)
            {
                colors = new Dictionary<string, Color>();
                foreach (var c in colorsList)
                    colors[c.ColorName] = c.Color;
            }
            return colors;
        }
    }

    public Color GetColorByName(string name) => Colors[name];
}

[System.Serializable]
internal struct ColorsTable
{
    public string ColorName;
    public Color Color;
}