using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshedPoints
{
    private List<Vector2> points = new List<Vector2>();
    private List<UIVertex> vertexes = new List<UIVertex>();
    public float roadWidth = 50f;
    private Vector2 screenOffset = new Vector2(Screen.width/2, Screen.height/2);


    public void Clear()
    {
        points.Clear();
        vertexes.Clear();
    }

    public void AddPoint(Vector2 pointPos)
    {
        Color color = Color.white;
        color.a = 1;

        points.Add(pointPos);
       

        int newPointIndex = points.Count - 1;

        Vector2 forward = Vector2.zero;
        if (newPointIndex < points.Count - 1)
        {
            forward += points[newPointIndex + 1] - points[newPointIndex];
        }
        if (points.Count > 1)
        {
            forward += points[newPointIndex] - points[newPointIndex - 1];
        }

        forward.Normalize();
        Vector2 left = new Vector2(-forward.y, forward.x);

        float completetionPercent = newPointIndex / (float)(points.Count - 1);

        UIVertex vertex1 = UIVertex.simpleVert;
        vertex1.position = points[newPointIndex] + left * roadWidth * 0.5f - screenOffset;
        vertex1.uv0 = new Vector2(0, completetionPercent);
        vertex1.color = color;
        vertexes.Add(vertex1);

        UIVertex vertex2 = UIVertex.simpleVert;
        vertex2.position = points[newPointIndex] - left * roadWidth * 0.5f - screenOffset;
        vertex2.uv0 = new Vector2(1, completetionPercent);
        vertex2.color = color;
        vertexes.Add(vertex2);


    }


    public int[] GetTris()
    {
        int[] tris = new int[2 * (points.Count - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < tris.Length; i++)
        {
            if (i < points.Count - 1)
            {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;

                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        return tris;
    }

    public int GetNumOfPoints()
    {
        return points.Count;
    }

    public List<UIVertex> GetUIVertices()
    {
        return vertexes;
    }
}
