using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RouletteFieldVisual : MonoBehaviour
{
    private RouletteWheelField field;
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RouletteColorTable colorTable;

    [SerializeField] private Material rimLightMaterial; 
    private Material originalMaterial;

    public void SetField(RouletteWheelField field)
    {
        this.field = field;

        field.ColorChanged += OnColorChanged;
        field.NumberChanged += OnNumberChanged;
        field.HighlightChanged += OnHighlightChanged;

        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
    }

    public void GenerateMesh(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void OnDestroy()
    {
        if (field != null)
        {
            field.ColorChanged -= OnColorChanged;
            field.NumberChanged -= OnNumberChanged;
            field.HighlightChanged -= OnHighlightChanged;
        }
    }

    public void OnNumberChanged(int number)
    {
        textMeshPro.text = number.ToString();
    }

    public void OnColorChanged(ColorType color)
    {
        meshRenderer.material.color = colorTable.GetColorByName(color);
    }

    public void OnHighlightChanged(bool isHighlighted)
    {
        if (isHighlighted && rimLightMaterial != null && originalMaterial != null)
        {
            meshRenderer.material = rimLightMaterial;
            meshRenderer.material.color = originalMaterial.color;
        }
        else if (originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
}