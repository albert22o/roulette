using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RouletteFieldVisual : MonoBehaviour
{
    private RouletteWheelField field;
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private RouletteColorTable colorTable;

    [SerializeField] private RingSegmentGenerator meshGenerator;

    public void SetField(RouletteWheelField field)
    {
        this.field = field;

        field.ColorChanged += OnColorChanged;
        field.NumberChanged += OnNumberChanged;
    }

    public void GenerateMesh(float innerRadius, float outerRadius, float height, float angle, int radialSegments)
    {
        meshGenerator.SetParameters(innerRadius, outerRadius, height, angle, radialSegments);
        meshGenerator.Generate();
    }


    private void OnDestroy()
    {
        field.ColorChanged -= OnColorChanged;
        field.NumberChanged -= OnNumberChanged;
    }

    public void OnNumberChanged(int number)
    {
        textMeshPro.text = number.ToString();
    }

    public void OnColorChanged(string color)
    {
        meshRenderer.material.color = colorTable.GetColorByName(color);
    }
}


