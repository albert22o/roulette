using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RouletteFieldVisual : MonoBehaviour
{
    private RouletteWheelField field;
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RouletteColorTable colorTable;
    [SerializeField] private Material highlightedMaterial;

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
        field.ColorChanged -= OnColorChanged;
        field.NumberChanged -= OnNumberChanged;
        field.HighlightChanged -= OnHighlightChanged;
    }

    public void OnNumberChanged(int number)
    {
        textMeshPro.text = number.ToString();
    }

    public void OnColorChanged(ColorType color)
    {
        meshRenderer.material.color = colorTable.GetColorByName(color);
    }

    public void OnHighlightChanged(bool isHighlited)
    {
        if(isHighlited &&highlightedMaterial != null)
            meshRenderer.material = highlightedMaterial;
        else if (originalMaterial != null)
            meshRenderer.material = originalMaterial;
    }
}


