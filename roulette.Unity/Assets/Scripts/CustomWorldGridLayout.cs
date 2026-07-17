using UnityEngine;

public class CustomWorldGridLayout : MonoBehaviour
{
    public enum Corner { UpperLeft, UpperRight, LowerLeft, LowerRight }
    public enum Axis { Horizontal, Vertical }
    public enum Constraint { Flexible, FixedColumnCount, FixedRowCount }
    public enum GridPlane { XY, XZ, YZ }

    [Header("Layout Settings")]
    public GridPlane gridPlane = GridPlane.XY;
    public Corner startCorner = Corner.UpperLeft;
    public Axis startAxis = Axis.Horizontal;
    public Constraint constraint = Constraint.Flexible;

    [Header("Alignment")]
    public bool centerHorizontal = false;
    public bool centerVertical = false;
    [Tooltip("Центрировать элементы в неполных рядах/колонках относительно общей ширины сетки")]
    public bool centerIncompleteRows = true;

    [Min(1)]
    public int constraintCount = 2;

    [Header("Dimensions")]
    public Vector2 cellSize = new Vector2(1, 1);
    public Vector2 spacing = new Vector2(0.2f, 0.2f);
    public Vector2 padding = new Vector2(0, 0);
    public float planeOffset = 0f;

    public bool autoUpdate = true;

    private int lastChildCount = -1;

    void OnValidate()
    {
        if (autoUpdate) UpdateLayout();
    }

    void LateUpdate()
    {
        if (!autoUpdate) return;

        int currentCount = transform.childCount;
        if (currentCount != lastChildCount)
        {
            lastChildCount = currentCount;
            UpdateLayout();
        }
    }

    public void UpdateLayout()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        int columns = 1;
        int rows = 1;

        switch (constraint)
        {
            case Constraint.FixedColumnCount:
                columns = Mathf.Max(1, constraintCount);
                rows = Mathf.CeilToInt((float)childCount / columns);
                break;

            case Constraint.FixedRowCount:
                rows = Mathf.Max(1, constraintCount);
                columns = Mathf.CeilToInt((float)childCount / rows);
                break;

            case Constraint.Flexible:
                columns = Mathf.CeilToInt(Mathf.Sqrt(childCount));
                rows = Mathf.CeilToInt((float)childCount / columns);
                break;
        }

        int actualCols = Mathf.Min(columns, childCount);
        int actualRows = rows;

        if (startAxis == Axis.Vertical)
        {
            actualRows = Mathf.Min(rows, childCount);
            actualCols = Mathf.CeilToInt((float)childCount / actualRows);
        }
        else
        {
            actualRows = Mathf.CeilToInt((float)childCount / actualCols);
        }

        float totalWidth = (actualCols * cellSize.x) + Mathf.Max(0, actualCols - 1) * spacing.x;
        float totalHeight = (actualRows * cellSize.y) + Mathf.Max(0, actualRows - 1) * spacing.y;

        Vector2 gridCenter = new Vector2(totalWidth * 0.5f, totalHeight * 0.5f);

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            int row = 0;
            int col = 0;

            if (startAxis == Axis.Horizontal)
            {
                row = i / columns;
                col = i % columns;
            }
            else
            {
                col = i / rows;
                row = i % rows;
            }

            // Вычисляем, сколько элементов в текущем ряду/колонке для выравнивания неполных линий
            int elementsInCurrentLine = actualCols;
            if (startAxis == Axis.Horizontal)
            {
                // Если это последний (неполный) ряд
                if (row == actualRows - 1)
                {
                    elementsInCurrentLine = childCount % columns;
                    if (elementsInCurrentLine == 0) elementsInCurrentLine = columns;
                }
            }
            else
            {
                // Если заполнение вертикальное, проверяем последнюю (неполную) колонку
                if (col == actualCols - 1)
                {
                    elementsInCurrentLine = childCount % rows;
                    if (elementsInCurrentLine == 0) elementsInCurrentLine = rows;
                }
            }

            Vector3 pos = CalculatePosition3D(row, col, gridCenter, elementsInCurrentLine, actualCols, actualRows);
            child.localPosition = pos;
        }
    }

    Vector3 CalculatePosition3D(int row, int col, Vector2 centerOffset, int elementsInLine, int maxCols, int maxRows)
    {
        float u = col * (cellSize.x + spacing.x);
        float v = row * (cellSize.y + spacing.y);

        // Смещение для неполных рядов (работает, даже если общая сетка НЕ центрирована)
        if (centerIncompleteRows)
        {
            if (startAxis == Axis.Horizontal && elementsInLine < maxCols)
            {
                float rowWidth = (elementsInLine * cellSize.x) + Mathf.Max(0, elementsInLine - 1) * spacing.x;
                float maxWidth = (maxCols * cellSize.x) + Mathf.Max(0, maxCols - 1) * spacing.x;
                u += (maxWidth - rowWidth) * 0.5f;
            }
            else if (startAxis == Axis.Vertical && elementsInLine < maxRows)
            {
                float colHeight = (elementsInLine * cellSize.y) + Mathf.Max(0, elementsInLine - 1) * spacing.y;
                float maxHeight = (maxRows * cellSize.y) + Mathf.Max(0, maxRows - 1) * spacing.y;
                v += (maxHeight - colHeight) * 0.5f;
            }
        }

        if (centerHorizontal)
        {
            u += cellSize.x * 0.5f - centerOffset.x;
        }
        else
        {
            u += padding.x;
        }

        if (centerVertical)
        {
            v += cellSize.y * 0.5f - centerOffset.y;
        }
        else
        {
            v += padding.y;
        }

        switch (startCorner)
        {
            case Corner.UpperLeft:
                v = -v;
                break;
            case Corner.UpperRight:
                u = -u;
                v = -v;
                break;
            case Corner.LowerRight:
                u = -u;
                break;
        }

        switch (gridPlane)
        {
            case GridPlane.XY:
                return new Vector3(u, v, planeOffset);
            case GridPlane.XZ:
                return new Vector3(u, planeOffset, v);
            case GridPlane.YZ:
                return new Vector3(planeOffset, u, v);
            default:
                return Vector3.zero;
        }
    }
}
