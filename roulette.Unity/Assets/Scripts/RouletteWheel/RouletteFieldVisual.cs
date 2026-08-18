using TMPro;
using UnityEngine;

public class RouletteFieldVisual : MonoBehaviour
{
    private RouletteWheelField field;
    private FieldHighlightState highlight;

    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RouletteColorTable colorTable;

    [SerializeField] private Material rimLightMaterial;
    [SerializeField] private Material numberBetMaterial;
    [SerializeField] private Material dozenBetMaterial;
    [SerializeField] private Material colorBetMaterial;

    private Material originalMaterial;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private Color currentRuntimeColor;

    public void SetField(RouletteWheelField field, FieldHighlightState highlight)
    {
        this.field = field;
        this.highlight = highlight;

        propertyBlock = new MaterialPropertyBlock();

        field.ColorChanged += OnColorChanged;
        field.NumberChanged += OnNumberChanged;
        highlight.BetHighlightLevelChanged += OnBetHighlightLevelChanged;
        highlight.RimHighlightChanged += OnRimHighlightChanged;

        if (meshRenderer != null)
            originalMaterial = meshRenderer.sharedMaterial;

        OnColorChanged(field.Color);
        OnNumberChanged(field.Number);
        UpdateVisualMaterial();
    }

    public void GenerateMesh(float angle) => transform.rotation = Quaternion.Euler(0, angle, 0);

    private void OnDestroy()
    {
        if (field != null)
        {
            field.ColorChanged -= OnColorChanged;
            field.NumberChanged -= OnNumberChanged;
        }
        if (highlight != null)
        {
            highlight.BetHighlightLevelChanged -= OnBetHighlightLevelChanged;
            highlight.RimHighlightChanged -= OnRimHighlightChanged;
        }
    }

    private void OnNumberChanged(int number) => textMeshPro.text = number.ToString();

    private void OnColorChanged(ColorType color)
    {
        currentRuntimeColor = colorTable.GetColorByName(color);
        ApplyColorToPropertyBlock();
    }

    private void OnBetHighlightLevelChanged(BetHighlightLevel level) => UpdateVisualMaterial();
    private void OnRimHighlightChanged(bool value) => UpdateVisualMaterial();

    private void UpdateVisualMaterial()
    {
        Material target =
            highlight.IsRimHighlighted ? rimLightMaterial :
            highlight.BetHighlightLevel switch
            {
                BetHighlightLevel.Number => numberBetMaterial,
                BetHighlightLevel.Dozen => dozenBetMaterial,
                BetHighlightLevel.Color => colorBetMaterial,
                _ => null
            };

        meshRenderer.sharedMaterial = target != null ? target : originalMaterial;

        ApplyColorToPropertyBlock();
    }

    private void ApplyColorToPropertyBlock()
    {
        if (meshRenderer == null || propertyBlock == null) return;

        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorPropertyId, currentRuntimeColor.linear);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }
}
