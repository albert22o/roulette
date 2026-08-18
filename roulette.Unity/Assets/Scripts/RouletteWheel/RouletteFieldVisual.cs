using System.Collections;
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
    [SerializeField] private float rimBlinkInterval = 0.3f;

    private Material originalMaterial;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private Color currentRuntimeColor;
    private Coroutine blinkRoutine;

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
    private void OnRimHighlightChanged(bool value)
    {
        if (value)
        {
            blinkRoutine ??= StartCoroutine(BlinkRimRoutine());
        }
        else
        {
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }
            UpdateVisualMaterial();
        }
    }

    

    private void ApplyColorToPropertyBlock()
    {
        if (meshRenderer == null || propertyBlock == null) return;

        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorPropertyId, currentRuntimeColor.linear);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private IEnumerator BlinkRimRoutine()
    {
        var wait = new WaitForSeconds(rimBlinkInterval);
        bool showRim = true;

        while (true)
        {
            meshRenderer.sharedMaterial = showRim ? rimLightMaterial : GetUnderlyingBetMaterial();
            ApplyColorToPropertyBlock();
            showRim = !showRim;
            yield return wait;
        }
    }
    private void UpdateVisualMaterial()
    {
        if (highlight.IsRimHighlighted)
        {
            ApplyColorToPropertyBlock();
            return;
        }

        meshRenderer.sharedMaterial = GetUnderlyingBetMaterial();
        ApplyColorToPropertyBlock();
    }

    private Material GetUnderlyingBetMaterial()
    {
        Material target = highlight.BetHighlightLevel switch
        {
            BetHighlightLevel.Number => numberBetMaterial,
            BetHighlightLevel.Dozen => dozenBetMaterial,
            BetHighlightLevel.Color => colorBetMaterial,
            _ => null
        };
        return target != null ? target : originalMaterial;
    }
}
