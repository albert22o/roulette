using UnityEngine;

[CreateAssetMenu(fileName = "ShopFontConfig", menuName = "Scriptable Objects/ShopFontConfig")]
public class ShopFontConfig : ScriptableObject
{
    [SerializeField] private Color canBuyColor = Color.green;
    [SerializeField] private Color cantBuyColor = Color.red;

    public string CanBuyColorHex => "#" + ColorUtility.ToHtmlStringRGB(canBuyColor);
    public string CantBuyColorHex => "#" + ColorUtility.ToHtmlStringRGB(cantBuyColor);

    public Color CanBuyColor => canBuyColor;
    public Color CantBuyColor => cantBuyColor;
}
