using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityData", menuName = "Scriptable Objects/RarityData")]
public class RarityData : ScriptableObject
{
    [SerializeField] private List<RarityDisplayData> raritiesList = new();

    private Dictionary<RarityType, RarityDisplayData> rarities;

    public Dictionary<RarityType, RarityDisplayData> Rarities
    {
        get
        {
            if (rarities == null)
            {
                rarities = new Dictionary<RarityType, RarityDisplayData>();
                foreach (var c in raritiesList)
                {
                    if (c != null && !rarities.ContainsKey(c.Type))
                        rarities[c.Type] = c;
                }
            }
            return rarities;
        }
    }

    public RarityDisplayData GetRarityDisplayData(RarityType type)
    {
        return Rarities.TryGetValue(type, out var data) ? data : null;
    }
}

[System.Serializable]
public class RarityDisplayData
{
    [SerializeField] private RarityType type;
    [SerializeField][TextArea(1, 3)] private string name;
    [SerializeField] private Color color = Color.white;

    public string Name => name;
    public RarityType Type => type;
    public Color Color => color;
}
