using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshedSpline
{


    public static MeshSpline GetGeneratedMesh(Vector2[] points, float roadWidth, Color color)
    {
        float defaultScreen = new Vector2Int(1080, 1920).magnitude;
        float currentScreen = new Vector2Int(Screen.width, Screen.height).magnitude;

        roadWidth = (roadWidth * currentScreen) / defaultScreen;

        MeshSpline meshSpline;
        meshSpline.uIVertices = new UIVertex[points.Length * 2];
        //Vector2[] uvs = new Vector2[verts.Length];
        meshSpline.tris = new int[2 * (points.Length - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {

            Vector2 forward = Vector2.zero;

            if (i < points.Length - 1)
            {
                forward += points[i + 1] - points[i];
            }
            if (i > 0)
            {
                forward += points[i] - points[i - 1];
            }
            forward.Normalize();
            Vector2 left = new Vector2(-forward.y, forward.x);
            meshSpline.uIVertices[vertIndex] = UIVertex.simpleVert;
            meshSpline.uIVertices[vertIndex + 1] = UIVertex.simpleVert;
            meshSpline.uIVertices[vertIndex].position = points[i] + left * roadWidth * 0.5f;
            meshSpline.uIVertices[vertIndex + 1].position = points[i] - left * roadWidth * 0.5f;

            meshSpline.uIVertices[vertIndex].color = color;
            meshSpline.uIVertices[vertIndex + 1].color = color;

            float completetionPercent = i / (float)(points.Length - 1);
            meshSpline.uIVertices[vertIndex].uv0 = new Vector2(0, completetionPercent);
            meshSpline.uIVertices[vertIndex + 1].uv0 = new Vector2(1, completetionPercent);

            if (i < points.Length - 1)
            {
                meshSpline.tris[triIndex] = vertIndex;
                meshSpline.tris[triIndex + 1] = vertIndex + 2;
                meshSpline.tris[triIndex + 2] = vertIndex + 1;

                meshSpline.tris[triIndex + 3] = vertIndex + 1;
                meshSpline.tris[triIndex + 4] = vertIndex + 2;
                meshSpline.tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        return meshSpline;
    }

}

public struct MeshSpline
{
    public UIVertex[] uIVertices;
    public int[] tris;

    
}