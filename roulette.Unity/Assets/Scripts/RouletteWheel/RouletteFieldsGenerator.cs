using UnityEngine;

public class RouletteFieldsGenerator : MonoBehaviour
{
    [Header("Параметры сегментов")]
    [SerializeField] private RouletteFieldVisual segmentPrefab;
    private int segmentsCount;

    [Header("Углы")]
    [SerializeField] private float gapAngle = 0.5f;

    [Header("Геометрия кольца")]
    [SerializeField] private float innerRadius = 0.8f;
    [SerializeField] private float outerRadius = 1.0f;
    [SerializeField] private float height = 0.2f;

    [Header("Качество")]
    [SerializeField] private int baseRadialSegments = 8;
    [SerializeField] private float detailMultiplier = 1.5f;
    [SerializeField] private int maxRadialSegments = 256;

    private RouletteFieldVisual[] segments;

    public void GenerateFields(RouletteWheelField[] fields)
    {
        this.segmentsCount = fields.Length;
        ClearOldSegments();

        if (segmentPrefab == null)
        {
            Debug.LogError("RouletteRingGenerator: Segment prefab is missing.");
        }

        segments = new RouletteFieldVisual[segmentsCount];

        float totalGap = gapAngle * segmentsCount;
        float segmentAngle = (360f - totalGap) / segmentsCount;

        int radialSegments = Mathf.Clamp(
            Mathf.RoundToInt(baseRadialSegments + segmentAngle * detailMultiplier),
            baseRadialSegments,
            maxRadialSegments
        );

        float stepAngle = segmentAngle + gapAngle;

        for (int i = 0; i < segmentsCount; i++)
        {
            RouletteFieldVisual segment = Instantiate(segmentPrefab, transform);
            segments[i] = segment;

            segment.GenerateMesh(innerRadius,
                outerRadius,
                height,
                segmentAngle,
                radialSegments);

            float angle = stepAngle * i;
            segment.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            segment.SetField(fields[i]);    
        }
    }

    private void ClearOldSegments()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
