using UnityEngine;

public class RouletteFieldsGenerator : MonoBehaviour
{
    [Header("Параметры сегментов")]
    [SerializeField] private RouletteFieldVisual segmentPrefab;
    private int segmentsCount;

    [Header("Углы")]
    [SerializeField] private float angleBetweenSegments = 0.5f;




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

        float totalGap = angleBetweenSegments * segmentsCount;
        float segmentAngle = (360f - totalGap) / segmentsCount;

        float stepAngle = segmentAngle + angleBetweenSegments;

        for (int i = 0; i < segmentsCount; i++)
        {
            RouletteFieldVisual segment = Instantiate(segmentPrefab, transform);
            segments[i] = segment;

            segment.GenerateMesh(segmentAngle);

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
