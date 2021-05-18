using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMeshRenderer : Graphic
{
    public MeshedPoints meshedPoints = new MeshedPoints();
    

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        BuildMesh(vh);
        
    }

    

    private void BuildMesh(VertexHelper vh)
    {

        if (meshedPoints.GetNumOfPoints() < 1)
        {
            return;
        }

        vh.AddUIVertexStream(meshedPoints.GetUIVertices(), new List<int>(meshedPoints.GetTris()));
    }

    public void UpdateMesh()
    {
        SetVerticesDirty();
    }

}


