using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingSegmentGenerator : MonoBehaviour
{
    private float innerRadius;
    private float outerRadius;
    private float height;
    private float angle;
    private int radialSegments;

    private Mesh mesh;

    public void SetParameters(
        float innerRadius,
        float outerRadius,
        float height,
        float angle,
        int radialSegments)
    {
        this.innerRadius = innerRadius;
        this.outerRadius = outerRadius;
        this.height = height;
        this.angle = angle;
        this.radialSegments = Mathf.Max(2, radialSegments);
    }

    public void Generate()
    {
        mesh = new Mesh();
        mesh.name = "RingSegment";

        BuildMesh();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void BuildMesh()
    {
        mesh.Clear(); // Очищаем меш перед генерацией

        // Для Flat Shading нам нужно по 6 вершин на каждый сегмент каждой стороны
        // (или по 4, если использовать общие индексы только внутри одной грани)
        // Но проще всего генерировать треугольники напрямую.

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        float radAngle = Mathf.Deg2Rad * angle;
        float halfH = height * 0.5f;

        for (int i = 0; i < radialSegments; i++)
        {
            float t0 = (float)i / radialSegments;
            float t1 = (float)(i + 1) / radialSegments;

            // сегмент от -angle/2 до +angle/2
            float startAngle = -radAngle * 0.5f;
            float a0 = startAngle + t0 * radAngle;
            float a1 = startAngle + t1 * radAngle;

            // Предрассчитываем позиции
            Vector3 inTop0 = new Vector3(innerRadius * Mathf.Cos(a0), halfH, innerRadius * Mathf.Sin(a0));
            Vector3 inTop1 = new Vector3(innerRadius * Mathf.Cos(a1), halfH, innerRadius * Mathf.Sin(a1));
            Vector3 outTop0 = new Vector3(outerRadius * Mathf.Cos(a0), halfH, outerRadius * Mathf.Sin(a0));
            Vector3 outTop1 = new Vector3(outerRadius * Mathf.Cos(a1), halfH, outerRadius * Mathf.Sin(a1));

            Vector3 inBot0 = inTop0; inBot0.y = -halfH;
            Vector3 inBot1 = inTop1; inBot1.y = -halfH;
            Vector3 outBot0 = outTop0; outBot0.y = -halfH;
            Vector3 outBot1 = outTop1; outBot1.y = -halfH;

            // Вспомогательная функция для добавления квада (4 вершины, 6 индексов)
            AddQuad(verts, uvs, tris, inTop0, inTop1, outTop1, outTop0, new Vector2(t0, 0), new Vector2(t1, 0), new Vector2(t1, 1), new Vector2(t0, 1)); // Top
            AddQuad(verts, uvs, tris, inBot0, outBot0, outBot1, inBot1, new Vector2(t0, 0), new Vector2(t0, 1), new Vector2(t1, 1), new Vector2(t1, 0)); // Bottom
            AddQuad(verts, uvs, tris, inBot0, inBot1, inTop1, inTop0, new Vector2(t0, 0), new Vector2(t1, 0), new Vector2(t1, 1), new Vector2(t0, 1)); // Inner Wall
            AddQuad(verts, uvs, tris, outBot0, outTop0, outTop1, outBot1, new Vector2(t0, 0), new Vector2(t0, 1), new Vector2(t1, 1), new Vector2(t1, 0)); // Outer Wall

            // Торцы (только в начале и в конце цикла)
            if (i == 0)
                AddQuad(verts, uvs, tris, inBot0, inTop0, outTop0, outBot0, Vector2.zero, Vector2.up, Vector2.one, Vector2.right);
            if (i == radialSegments - 1)
                AddQuad(verts, uvs, tris, inBot1, outBot1, outTop1, inTop1, Vector2.zero, Vector2.right, Vector2.one, Vector2.up);
        }




        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();

   

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Утилита для добавления полигона из 4-х точек
    private void AddQuad(List<Vector3> v, List<Vector2> u, List<int> t, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 u0, Vector2 u1, Vector2 u2, Vector2 u3)
    {
        int index = v.Count;
        v.AddRange(new[] { v0, v1, v2, v3 });
        u.AddRange(new[] { u0, u1, u2, u3 });
        t.AddRange(new[] { index, index + 1, index + 2, index, index + 2, index + 3 });
    }

}