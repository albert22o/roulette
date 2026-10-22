using UnityEngine;
using UnityEngine.Localization;
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
    [SerializeField] private LocalizedString colorName;
    [SerializeField] private LocalizedString colorDescription;

    public string ColorName => colorName.Resolve();
    public string ColorDescription => colorDescription.Resolve();
}
