using TMPro;
using UnityEngine;

public class BetFieldVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer bgRenderer;
    [SerializeField] private TextMeshPro text;

    [Header("Materials")]
    [SerializeField] Material blackMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material colorHighlight;
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material greyMaterial;

    public ColorType Color { get; private set; }
    public int Number { get; private set; }

    public void SetColor(ColorType color)
    {
        Color = color;
        bgRenderer.material = color switch
        {
            ColorType.Red => redMaterial,
            ColorType.Black => blackMaterial,
            ColorType.Green => greenMaterial,
            ColorType.White => whiteMaterial,
            ColorType.Grey => greyMaterial,
            _ => greenMaterial,
        };
    }

    public void SetNumber(int num)
    {
        Number = num;
        this.text.text = num.ToString();
    }

    public void Highlight(bool isEnabled)
    {
        if (isEnabled)
        {
            bgRenderer.material = colorHighlight;
        }
        else
        {
            bgRenderer.material = GetColorMaterial(Color);
        }
    }

    private Material GetColorMaterial(ColorType fieldColor) => fieldColor switch
    {
        ColorType.Red => redMaterial,
        ColorType.Black => blackMaterial,
        ColorType.Green => greenMaterial,
        ColorType.White => whiteMaterial,
        ColorType.Grey => greyMaterial,
        _ => greenMaterial,
    };
}
