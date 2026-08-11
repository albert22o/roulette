using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "ColorsDescription", menuName = "Scriptable Objects/ColorsDescription")]
public class ColorsDescription : ScriptableObject
{
    [SerializeField]
    private SerializedDictionary<ColorType, ColorDescriptionData> description;

    public ColorDescriptionData GetColorData(ColorType colorType) => description.TryGetValue(colorType, out var colorData) ? colorData : null;
}

[System.Serializable]
public class ColorDescriptionData
{
    [SerializeField][TextArea(1,3)] private string colorName;
    [SerializeField][TextArea(1,3)] private string colorDescription;

    public string ColorName => colorName;
    public string ColorDescription => colorDescription;
}
